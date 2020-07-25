using arx.Extract.BackgroundTasks.Core;
using arx.Extract.BackgroundTasks.Events;
using arx.Extract.Data.Entities;
using arx.Extract.Data.Repository;
using arx.Extract.Lib;
using arx.Extract.Types;
using AutoMapper;
using EventBus.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace arx.Extract.BackgroundTasks.Tasks
{
    public class ScheduledArchiveService : BackgroundService
    {
        private readonly BackgroundTaskSettings _settings;
        private readonly IMapper _mapper;
        private readonly IEventBus _eventBus;
        private readonly ILogger<ScheduledArchiveService> _logger;
        private readonly ISubjectRepository _subjectRepo;
        private readonly IJobRepository _jobRepository;
        private readonly IJobItemRepository _jobItemRepository;
        private readonly IFulfilmentRepository _fulfilmentRepository;
        private readonly IFulfilmentItemRepository _fulfilmentItemRepository;
        private readonly IPublicationRepository _publicationRepository;
        private readonly IArchiveFetch _archiveFetch;
        private readonly ITransformService _transformService;

        public ScheduledArchiveService(IOptions<BackgroundTaskSettings> settings,
            IMapper mapper,
            IEventBus eventBus,
            ILogger<ScheduledArchiveService> logger,
            ISubjectRepository subjectRepo,
            IJobRepository jobRepository,
            IJobItemRepository jobItemRepository,
            IFulfilmentRepository fulfilmentRepository,
            IFulfilmentItemRepository fulfilmentItemRepository,
            IPublicationRepository publicationRepository,
            IArchiveFetch archiveFetch,
            ITransformService transformService)
        {
            _settings = settings?.Value ?? throw new ArgumentException(nameof(settings));
            _mapper = mapper;
            _eventBus = eventBus;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subjectRepo = subjectRepo;
            _jobRepository = jobRepository;
            _jobItemRepository = jobItemRepository;
            _fulfilmentRepository = fulfilmentRepository;
            _fulfilmentItemRepository = fulfilmentItemRepository;
            _publicationRepository = publicationRepository;
            _archiveFetch = archiveFetch;
            _transformService = transformService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("ScheduledArchiveFetchService background task is starting.");

            await DoWork(stoppingToken);
        }
        private async Task DoWork(CancellationToken stoppingToken)
        {
            ServiceSetup();

            stoppingToken.Register(() => _logger.LogDebug("#1 ScheduledArchiveFetchService background task is stopping."));

            var extractionMode = _settings.ExtractionMode;

            if (extractionMode != ExtractTypeEnum.Archive.ToString())
            {
                _logger.LogInformation($"Stopping {this.GetType().Name} because current Extraction Mode is : {extractionMode}");
                await StopAsync(stoppingToken);
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug($"ScheduledArchiveFetchService background task is doing background work. [{DateTime.UtcNow}]");

                await RunArchiveExtrationService(stoppingToken);

                _logger.LogInformation($"_settings.PostFetchWaitTime : {_settings.PostFetchWaitTime}");

                await Task.Delay(_settings.PostFetchWaitTime, stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("ScheduledArchiveFetchService background task is stopping.");
            await Task.CompletedTask;
        }

        private async Task RunArchiveExtrationService(CancellationToken stoppingToken)
        {
            _logger.LogDebug("Running Scheduled Archive Service.");

            var newFulfilmentId = await ExecuteExtraction(stoppingToken);

            var extractionCompletedEvent = new ExtractionCompletedIntegrationEvent(newFulfilmentId.ToString());

            _logger.LogInformation("----- Publishing Integration Event: {IntegrationEventId} from {AppName} = ({@IntegrationEvent})",
                                    extractionCompletedEvent.ExtractionId, Program.AppName, extractionCompletedEvent);

            //TODO: Publish to eventBus
            _eventBus.Publish(extractionCompletedEvent);
          }

        private async Task<Guid> ExecuteExtraction(CancellationToken stoppingToken)
        {
            _logger.LogDebug("Reading Metadata to determine archive task parameters.");

            JobEntity job = _jobRepository.GetJob(ExtractTypeEnum.Archive, _settings.ArchiveJobName);

            List<JobItemEntity> jobItems = _jobItemRepository.GetJobItems(job.UniqueName);

            if (jobItems == null || jobItems.Count == 0)
            {
                //TODO:Retry, fail logic
            }

            FulfilmentEntity lastFulfilment = _fulfilmentRepository.GetLastFulfilment(job.UniqueName);

            _logger.LogInformation($"Creating new Fulfilment record from Job {job.UniqueName}");
            int minQueryDateInterval = (int)Math.Floor(jobItems.Average(x => x.QueryDateInterval));

            FulfilmentEntity newFulfilment = CreateNewFulfilment(job, lastFulfilment, minQueryDateInterval);
            _logger.LogInformation($"Created New Fulfilment {newFulfilment.JobName} -[{newFulfilment.FulfilmentId}] -  @{newFulfilment.JobStartedDate}");
            _logger.LogInformation($"New Fulfilment -[{newFulfilment.FulfilmentId}] - QueryFrom [{newFulfilment.QueryFromDate}] to [{newFulfilment.QueryToDate}]");

            if (newFulfilment.FulfilmentId == null)
            {
                //TODO: retry logic if Table Storage Insert failed.
            }

            List<FulfilmentItemEntity> newfulfilmentItems = new List<FulfilmentItemEntity>();

            foreach (var jobItem in jobItems)
            {
                List<Tuple<DateTime, DateTime>> requestDateIntervals = GetRequestChunkedDates(lastFulfilment, jobItem.QueryDateInterval);

                foreach (var interval in requestDateIntervals)
                {
                    newfulfilmentItems.Add(CreateNewFulfilmentItem(jobItem, interval, job.QueryBaseUrl, newFulfilment.FulfilmentId));
                }
            }

            List<FulfilmentItemEntity> fulfilmentItems = _fulfilmentItemRepository.SaveFulfilmentItems(newfulfilmentItems);

            if (fulfilmentItems.Count > 0)
            {
                _logger.LogInformation($"Created [{fulfilmentItems.Count}] New Fulfilment Items from Fulfilment {newFulfilment.FulfilmentId} - @{DateTime.UtcNow}");
            }
            else
            {
                _logger.LogError($"Error persisting [{fulfilmentItems.Count}] New Fulfilment Items from Fulfilment {newFulfilment.FulfilmentId} - @{DateTime.UtcNow}");
                //TODO: retry logic if Table Storage Insert failed.
            }

            Stopwatch stopwatch = new Stopwatch();
            //Run Http Request, transform, persist operations per fulfilment Item
            foreach (var fulfilmentItem in fulfilmentItems)
            {
                List<ArxivItem> allResults = new List<ArxivItem>();

                fulfilmentItem.JobItemStartDate = DateTime.UtcNow;

                //Make initial http request to external website/API          
                stopwatch.Start();
                var (initialResponse, initialItems) = _archiveFetch.GetArxivItems(fulfilmentItem.Url).Result;
                stopwatch.Stop();

                //Save http request time elapsed.
                fulfilmentItem.HttpRequestIsSuccess = initialResponse.IsSuccessStatusCode;
                fulfilmentItem.FetchTimeSpan = stopwatch.ElapsedMilliseconds;
                stopwatch.Reset();

                fulfilmentItem.HttpRequestCount++;

                //Add response to results list
                allResults.Add(initialItems);

                int totalAvialable = initialItems.totalResults;
                int fetched = initialItems.itemsPerPage;
                string initialUrl = fulfilmentItem.Url;

                fulfilmentItem.TotalResults = totalAvialable;
                fulfilmentItem.ResultSizePerHttpRequest = initialItems.itemsPerPage;

                if (fetched < totalAvialable)
                {
                    _logger.LogInformation($"FulfilmentItem {fulfilmentItem.ItemUId} - Only fetched[{fetched}] out of [{totalAvialable}]. Making further Paged Http Requests...");

                    //Calculate the number of requests required to fetch all items for the initial query
                    int requests = (int)Math.Floor((double)(fetched / totalAvialable)) - 1;

                    //Get delay value from settings/Environment
                    int delay = _settings.ArxivApiPagingRequestDelay;

                    //Record delay value
                    fulfilmentItem.DelayBetweenHttpRequests = delay;

                    for (int i = 0; i < requests; i++)
                    {
                        //Apply delay, as per the request by Arxiv.org api access policy.
                        await Task.Delay(delay * 1000);

                        //Calculate current start index value
                        int currentStartIndex = (i + 1) * fetched + 1;

                        //Add next start index to request url
                        string pagedUrl = $"{initialUrl}&start={currentStartIndex}";

                        _logger.LogInformation($"fulfilmentItem {fulfilmentItem.ItemUId} - Making paged Http request no [{i + 1}]");
                        stopwatch.Start();
                        var (pagedResponse, pagedItems) = _archiveFetch.GetArxivItems(pagedUrl).Result;
                        stopwatch.Stop();

                        //Add meta data
                        fulfilmentItem.FetchTimeSpan += stopwatch.ElapsedMilliseconds;
                        stopwatch.Reset();
                        fulfilmentItem.HttpRequestCount++;
                        fulfilmentItem.HttpRequestIsSuccess = pagedResponse.IsSuccessStatusCode;

                        //Add reponse to result list
                        allResults.Add(pagedItems);
                    }
                }

                //Re-Set FetchTimeSpan as time taken handling Http requests, if there have been Paged/more than one requests.
                if (fulfilmentItem.DelayBetweenHttpRequests > 0)
                {
                    fulfilmentItem.FetchTimeSpan = (fulfilmentItem.JobItemStartDate - DateTime.UtcNow).TotalMilliseconds;
                }

                fulfilmentItem.TotalResults = allResults?.Sum(x => x.EntryList?.Count) ?? 0;
                newFulfilment.TotalCount += fulfilmentItem.TotalResults;

                //Tranform and Persist to Storage
                if (fulfilmentItem.TotalResults < 1)
                {
                    _logger.LogInformation($"FulfilmentItem {fulfilmentItem.ItemUId} - returned 0 items.");
                }
                else
                {
                    List<Entry> allArxivEntries = new List<Entry>();

                    //Accumulate all Arxiv Entry values
                    allResults?.ForEach(x => allArxivEntries.AddRange(x.EntryList));

                    if (allArxivEntries.Count == 0)
                    {
                        fulfilmentItem.DataExtractionIsSuccess = true;
                    }
                    else
                    {
                        //Perform transformations
                        var publications = _transformService.TransformArxivEntriesToPublications(allArxivEntries);

                        //Set type transformaiton success
                        fulfilmentItem.DataExtractionIsSuccess = (publications.Count == allArxivEntries.Count);

                        //Set fulfilment Ids
                        var entityList = _mapper.Map<List<PublicationItemEntity>>(publications);
                        entityList?.ForEach(e =>
                        {
                            e.PartitionKey = newFulfilment.FulfilmentId.ToString();
                            e.FulfilmentId = newFulfilment.FulfilmentId.ToString();
                            e.FulFilmentItemId = fulfilmentItem.ItemUId.ToString();
                        });

                        //Persist publications to Storage - Batch insert
                        int saved = await _publicationRepository.BatchSavePublications(entityList);
                        
                        if (saved == 0)
                        {
                            _logger.LogError($"FulfilmentItem {fulfilmentItem.ItemUId} - persisted 0 publications to Storage");
                        }
                    }
                }

                //Record process completion Time, interval               
                fulfilmentItem.JobItemCompletedDate = DateTime.UtcNow;
                fulfilmentItem.TotalProcessingInMilliseconds =
                    (fulfilmentItem.JobItemCompletedDate - fulfilmentItem.JobItemStartDate).TotalMilliseconds;

                //Save to fulfilment Item to database
                var savedItem = _fulfilmentItemRepository.SaveFulfilmentItem(fulfilmentItem);

                //Log Fulfilment Item summary
                string logFulfilmentItem = $"FulfilmentItem [{fulfilmentItem.ItemUId}] - SubjectCode[{fulfilmentItem.QuerySubjectCode}] - Started @{fulfilmentItem.JobItemStartDate} - Completed @{fulfilmentItem.JobItemCompletedDate} - Fetched ={fulfilmentItem.TotalResults}";
               
                if (savedItem != null)
                    _logger.LogInformation(logFulfilmentItem);
                else
                    _logger.LogError($"Error Saving {logFulfilmentItem}");
            }

            //Save new Fulilment record values
            newFulfilment.PartialSuccess = fulfilmentItems.Any(x => x.HttpRequestIsSuccess == true)
               && fulfilmentItems.Any(x => x.DataExtractionIsSuccess = true);

            newFulfilment.CompleteSuccess = fulfilmentItems.All(x => x.HttpRequestIsSuccess == true)
                && fulfilmentItems.All(x => x.DataExtractionIsSuccess = true);

            newFulfilment.JobCompletedDate = DateTime.UtcNow;            
            newFulfilment.ProcessingTimeInSeconds = (newFulfilment.JobCompletedDate - newFulfilment.JobStartedDate).TotalSeconds;

            //Persist to Storage
            var savedNew = await _fulfilmentRepository.SaveFulfilment(newFulfilment);

            if (savedNew != null)
            {
                _logger.LogInformation($"Fulfilment {newFulfilment.JobName} -[{newFulfilment.FulfilmentId}] - Completed @{newFulfilment.JobCompletedDate} - Total Count = {newFulfilment.TotalCount} - From [{ newFulfilment.QueryFromDate}] To [{ newFulfilment.QueryToDate}]");
            }
            else
            {
                _logger.LogInformation($@"Error Saving - Fulfilment {newFulfilment.JobName} -[{newFulfilment.FulfilmentId}] - Completed @{newFulfilment.JobCompletedDate} - Total Count = {newFulfilment.TotalCount} - From [{ newFulfilment.QueryFromDate}] To [{ newFulfilment.QueryToDate}]");
            }

            return newFulfilment.FulfilmentId;
        }

        /// <summary>
        /// Returns a list of From, To datetime tuple values.
        /// Returns a list of calcualted  From, To date tuple values, if the given query Date interval compared to the last fulfilment interval is not optimal.
        /// Otherwise resturns one set of (From,To) dates callcuated from the given queryDate Interval.
        /// </summary>
        /// <param name="lastFulfilment">FulfilmentEntity</param>
        /// <param name="queryDateInterval">int</param>
        /// <returns>List<Tuple<DateTime, DateTime>></returns>
        private List<Tuple<DateTime, DateTime>> GetRequestChunkedDates(FulfilmentEntity lastFulfilment, int queryDateInterval)
        {
            /* Tuple(FromDate,ToDate) */
            List<Tuple<DateTime, DateTime>> result = new List<Tuple<DateTime, DateTime>>();

            var (lastSpanDays, nextFromDate, nextToDate) = CalculateQueryDates(lastFulfilment, queryDateInterval);

            /* queryDateInterval is optimal */
            if (queryDateInterval >= lastSpanDays)
            {
                result.Add(new Tuple<DateTime, DateTime>(nextFromDate, nextToDate));
                return result;
            }

            /* lastSpanDays is too big, needs to be chuncked. Requires more than one set of (from, to) query dates / fulfilment items */

            int mod = lastSpanDays % queryDateInterval;
            int chunkSize = (int)Math.Floor((double)lastSpanDays / queryDateInterval);
            int chunks = mod != 0 ? ((queryDateInterval - mod) / lastSpanDays) + 1 : queryDateInterval / lastSpanDays;

            /* Create Query date interval Chucks*/
            for (int i = 0; i < chunks; i++)
            {
                var toDate = i == 0 ? nextToDate : result[i].Item1.AddDays(-1);
                var fromDate = toDate.AddDays(-1 * chunkSize);
                result.Add(new Tuple<DateTime, DateTime>(fromDate, toDate));
            }

            /* If chunk count is an odd number, add the last Query date interval Chuck */
            if (mod != 0)
            {
                var lastToDate = result.Last().Item1.AddDays(-1);
                var lastFromDate = lastToDate.AddDays(-1 * mod);
                result.Add(new Tuple<DateTime, DateTime>(lastFromDate, lastToDate));
            }

            return result;
        }

        private FulfilmentItemEntity CreateNewFulfilmentItem(JobItemEntity jobItem, Tuple<DateTime, DateTime> requestDateInterval, string baseUrl, Guid newFulfilmentId)
        {
            FulfilmentItemEntity fulfilmentItem = new FulfilmentItemEntity()
            {
                FulfilmentId = newFulfilmentId, //PK
                ItemUId = Guid.NewGuid(), //RK
                JobItemId = jobItem.JobItemId,
                QuerySubjectCode = jobItem.QuerySubjectCode,
                QuerySubjectGroup = jobItem.QuerySubjectGroup,
                ItemsPerRequest = jobItem.ItemsPerRequest,
                JobItemStartDate = new DateTime(1970, 01, 01),
                JobItemCompletedDate = new DateTime(1970, 01, 01),
            };

            fulfilmentItem.PartitionKey = fulfilmentItem.FulfilmentId.ToString();
            fulfilmentItem.RowKey = fulfilmentItem.ItemUId.ToString();
            fulfilmentItem.Url = ConstructRequestUrl(requestDateInterval, baseUrl, fulfilmentItem);

            return fulfilmentItem;
        }

        private string ConstructRequestUrl(Tuple<DateTime, DateTime> requestDateInterval, string baseUrl, FulfilmentItemEntity fulfilmentItem)
        {
            UrlParams urlParams = new UrlParams()
            {
                SubjectCode = fulfilmentItem.QuerySubjectCode,
                SubjectGroupCode = fulfilmentItem.QuerySubjectGroup,
                QueryFromDate = requestDateInterval.Item1,
                QueryToDate = requestDateInterval.Item2,
                ItemsPerRequest = fulfilmentItem.ItemsPerRequest,
                QueryBaseUrl = baseUrl
            };

            return UrlMaker.FulfilmentUrlBetweenDates(urlParams);
        }

        private FulfilmentEntity CreateNewFulfilment(JobEntity job, FulfilmentEntity lastFulfilment, int averageQueryDateInterval)
        {
            FulfilmentEntity item = new FulfilmentEntity()
            {
                JobName = job.UniqueName,
                FulfilmentId = Guid.NewGuid(),
                Type = job.Type,
                JobStartedDate = DateTime.UtcNow,
                JobCompletedDate = new DateTime(1970, 01, 01)
            };

            item.PartitionKey = item.JobName;
            item.RowKey = item.FulfilmentId.ToString();

            var (_, fromDate, toDate) = CalculateQueryDates(lastFulfilment, averageQueryDateInterval);

            item.QueryFromDate = fromDate;
            item.QueryToDate = toDate;

            return _fulfilmentRepository.SaveFulfilment(item).Result;
        }

        private static (int, DateTime, DateTime) CalculateQueryDates(FulfilmentEntity lastFulfilment, int queryDateInterval)
        {
            DateTime queryFromDate;
            DateTime queryToDate;

            if (lastFulfilment == null || lastFulfilment.QueryFromDate == DateTime.MinValue)
            {
                queryToDate = DateTime.UtcNow.AddDays(-2).Date;
                queryFromDate = queryToDate.AddDays(-1 * queryDateInterval).Date;
                return (queryDateInterval, queryFromDate, queryToDate);
            }

            TimeSpan? span = (lastFulfilment?.QueryToDate - lastFulfilment?.QueryFromDate);
            int lastFulfilmentSpanDays = span.HasValue ? Math.Abs(span.Value.Days) : 0;
            queryToDate = lastFulfilment.QueryFromDate.AddDays(-1).Date;
            queryFromDate = queryToDate.AddDays(-1 * lastFulfilmentSpanDays).Date;

            return (lastFulfilmentSpanDays, queryFromDate, queryToDate);
        }

        private void ServiceSetup()
        {
            //Seed Subjects if empty
            if (_subjectRepo.HasSeed() == false)
            {
                _logger.LogInformation($"Seeding {_subjectRepo.TableName()} Table.");

                if (_subjectRepo.SeedSubjects())
                    _logger.LogInformation($"Successfullly Inserted Seed data to  {_subjectRepo.TableName()} Table.");
                else
                    _logger.LogDebug($"Error inserting Seed data to  {_subjectRepo.TableName()}  Table");
            }

            //Seed Jobs if Empty
            if (_jobRepository.HasSeed() == false)
            {
                _logger.LogInformation($"Seeding {_jobRepository.TableName()}  Table.");
                if (_jobRepository.SeedJobs())
                    _logger.LogInformation($"Successfullly Inserted Seed data to {_jobRepository.TableName()} Table.");
                else
                    _logger.LogDebug($"Error inserting Seed data to  {_jobRepository.TableName()}  Table");
            }

            //Seed JobItems if Empty
            if (_jobItemRepository.HasSeed() == false)
            {
                _logger.LogInformation($"Seeding {_jobItemRepository.TableName()}  Table.");
                if (_jobItemRepository.SeedJobItems())
                    _logger.LogInformation($"Successfullly Inserted Seed data to {_jobItemRepository.TableName()} Table.");
                else
                    _logger.LogDebug($"Error inserting Seed data to  {_jobItemRepository.TableName()} Table");
            }
        }
    }
}
