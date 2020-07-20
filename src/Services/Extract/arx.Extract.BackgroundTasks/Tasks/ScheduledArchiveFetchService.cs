﻿using arx.Extract.BackgroundTasks.Core;
using arx.Extract.BackgroundTasks.Events;
using arx.Extract.BackgroundTasks.Types;
using arx.Extract.Data.Entities;
using arx.Extract.Data.Repository;
using arx.Extract.Lib;
using arx.Extract.Types;
using EventBus.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace arx.Extract.BackgroundTasks.Tasks
{

    public class ScheduledArchiveService : BackgroundService
    {
        private readonly BackgroundTaskSettings _settings;
        private readonly IEventBus _eventBus;
        private readonly ILogger<ScheduledArchiveService> _logger;
        private readonly ISubjectRepository _subjectRepo;
        private readonly IJobRepository _jobRepository;
        private readonly IJobItemRepository _jobItemRepository;
        private readonly IFulfilmentRepository _fulfilmentRepository;
        private readonly IFulfilmentItemRepository _fulfilmentItemRepository;
        private readonly IPublicationRepository _publicationRepository;
        private readonly IArchiveFetch _archiveFetch;

        public ScheduledArchiveService(IOptions<BackgroundTaskSettings> settings,
            IEventBus eventBus,
            ILogger<ScheduledArchiveService> logger,
            ISubjectRepository subjectRepo,
            IJobRepository jobRepository,
            IJobItemRepository jobItemRepository,
            IFulfilmentRepository fulfilmentRepository,
            IFulfilmentItemRepository fulfilmentItemRepository,
            IPublicationRepository publicationRepository,
            IArchiveFetch archiveFetch)
        {
            _settings = settings?.Value ?? throw new ArgumentException(nameof(settings));
            _eventBus = eventBus;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subjectRepo = subjectRepo;
            _jobRepository = jobRepository;
            _jobItemRepository = jobItemRepository;
            _fulfilmentRepository = fulfilmentRepository;
            _fulfilmentItemRepository = fulfilmentItemRepository;
            _publicationRepository = publicationRepository;
            _archiveFetch = archiveFetch;
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

                RunArchiveExtrationService(stoppingToken);

                _logger.LogInformation($"_settings.PostFetchWaitTime : {_settings.PostFetchWaitTime}");

                await Task.Delay(_settings.PostFetchWaitTime, stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("ScheduledArchiveFetchService background task is stopping.");
            await Task.CompletedTask;
        }

        private void RunArchiveExtrationService(CancellationToken stoppingToken)
        {
            _logger.LogDebug("Running Scheduled Archive Service.");

            ExecuteExtraction(stoppingToken);

            //TODO connec to db read last archive task, determine next task's jobs' params

            var extractId = Guid.NewGuid().ToString();

            _logger.LogDebug("Fetching data from Http requests and persisting to Store.");
            //TODO  Loop through each subjectcode/url request cycle


            var extractionCompletedEvent = new ExtractionCompletedIntegrationEvent(extractId);

            _logger.LogInformation("----- Publishing Integration Event: {IntegrationEventId} from {AppName} = ({@IntegrationEvent})",
                                    extractionCompletedEvent.ExtractionId, Program.AppName, extractionCompletedEvent);

            //TODO: Publish to eventBus
            _eventBus.Publish(extractionCompletedEvent);
        }

        public void ExecuteExtraction(CancellationToken stoppingToken)
        {
            _logger.LogDebug("Reading Metadata to determine archive task parameters.");
            //  _repo.GetLastJob();

            var jobName = _settings.ArchiveJobName ?? ExtractTypeEnum.Archive.ToString();
            JobEntity job = _jobRepository.GetJobByExtractonMode(jobName);
            List<JobItemEntity> jobItems = _jobItemRepository.GetJobItems(job.JobId);

            if (jobItems == null || jobItems.Count == 0)
            {
                //TODO:Retry, fail logic
            }

            FulfilmentEntity lastFulfilment = _fulfilmentRepository.GetLastJobRecord();

            _logger.LogInformation($"Creating new JobRecord Entry from Job {job.JobId}");

            FulfilmentEntity newFulfilment = CreateNewFulfilment(job, lastFulfilment);
            if (newFulfilment.FulfilmentId == null)
            {
                //TODO: retry logic
            }

            List<FulfilmentItemEntity> newfulfilmentItems = new List<FulfilmentItemEntity>();

            foreach (var jobItem in jobItems)
            {
                List<Tuple<DateTime, DateTime>> requestDateIntervlas =
                    GetRequestChunkedDates(lastFulfilment, jobItem.QueryDateInterval);

                foreach (var interval in requestDateIntervlas)
                {
                    newfulfilmentItems.Add(CreateNewFulfilmentItem(jobItem, lastFulfilment, interval, job.QueryBaseUrl));
                }
            }

            List<FulfilmentItemEntity> fulfilmentItems = _fulfilmentItemRepository.SaveFulfilmentItems(newfulfilmentItems);

            //Run Http Request, transform, persist operations per fulfilment Item

            foreach (var item in fulfilmentItems)
            {
                List<ArxivItem> allResults = new List<ArxivItem>();
                item.JobItemStartDate = DateTime.UtcNow;

                //Make initial http request
                ArxivItem initialItems = new ArxivItem();
                HttpResponseMessage initialResponse;
                (initialResponse, initialItems) = _archiveFetch.GetArxivItems(item.Url).Result;

                item.HttpRequestIsSuccess = initialResponse.IsSuccessStatusCode;
                item.FetchTimeSpan += initialResponse.Headers.Age.Value.TotalMilliseconds;

                item.HttpRequestCount++;
                //Add reponse to result list
                allResults.Add(initialItems);

                int total = initialItems.totalResults;
                int fetched = initialItems.itemsPerPage;
                string initialUrl = item.Url;

                item.TotalResults = total;
                item.ResultSizePerHttpRequest = initialItems.itemsPerPage;

                if (fetched < total)
                {
                    //Calculate the number of requests required to fetch all items for the initial query
                    int requests = (int)Math.Floor((double)(fetched / total)) - 1;

                    //Get delay value from settings/Environment
                    int delay = _settings.ArxivApiPagingRequestDelay;

                    //Record delay value
                    item.DelayBetweenHttpRequests = delay;

                    for (int i = 0; i < requests; i++)
                    {
                        //Apply delay, as per the request by Arxiv.org api access policy.
                        Task.Delay(delay * 1000);

                        //Calculate current start index value
                        int currentStartIndex = (i + 1) * fetched + 1;

                        //Add next start index to request url
                        string pagedUrl = $"{initialUrl}&start={currentStartIndex}";

                        ArxivItem pagedItems = new ArxivItem();
                        HttpResponseMessage pagedResponse;
                        (pagedResponse, pagedItems) = _archiveFetch.GetArxivItems(pagedUrl).Result;

                        item.FetchTimeSpan += pagedResponse.Headers.Age.Value.TotalMilliseconds;
                        item.HttpRequestCount++;

                        if (item.HttpRequestIsSuccess)
                        {
                            item.HttpRequestIsSuccess = pagedResponse.IsSuccessStatusCode;
                        }

                        //Add reponse to result list
                        allResults.Add(pagedItems);
                    }
                }

                //Recored time taken handling Http requests
                item.FetchTimeSpan = (item.JobItemStartDate - DateTime.UtcNow).TotalMilliseconds;

                int totalEntries = allResults?.Sum(x => x.itemsPerPage) ?? 0;
                item.TotalResults = totalEntries;

                if (item.TotalResults > 0)
                {
                    //Trasform entries to Publications
                    List<Entry> allArxivEntries = new List<Entry>();
                    allResults?.ForEach(x => allArxivEntries.AddRange(x.EntryList));

                    List<PublicationItemEntity> publications = 
                        TransformToPublications.TransformArxivEntriesToPublications(allArxivEntries);

                    //Persist to database
                    item.DataExtractionIsSuccess = publications.Count == allArxivEntries.Count;

                    //Persist publications to database - Batch insert
                    _publicationRepository.BatchSavePublications(publications);

                }

                //Record process completion Time, interval
                item.JobItemCompletedDate = DateTime.UtcNow;
                item.TotalProcessingInMilliseconds = (item.JobItemStartDate - item.JobItemCompletedDate).TotalMilliseconds;

                //Save to fulfilment Item to database
                _fulfilmentItemRepository.SaveFulfilmentItem(item);
            }
        }

       

        private List<Tuple<DateTime, DateTime>> GetRequestChunkedDates(FulfilmentEntity lastFulfilment, int queryDateInterval)
        {
            /* Tuple(FormDate,ToDate) */
            List<Tuple<DateTime, DateTime>> result = new List<Tuple<DateTime, DateTime>>();

            TimeSpan? span = (lastFulfilment?.QueryToDate - lastFulfilment?.QueryFromDate);
            int lastSpanDays = span.HasValue ? span.Value.Days : 0;

            DateTime nextToDate = lastFulfilment.QueryFromDate.AddDays(-1);
            DateTime nextFromDate = nextToDate.AddDays(-1 * queryDateInterval);

            if (queryDateInterval >= lastSpanDays)
            {
                /* queryDateInterval is optimal */
                result.Add(new Tuple<DateTime, DateTime>(nextFromDate, nextToDate));
            }
            else if (queryDateInterval < lastSpanDays)
            {
                /* lastSpanDays is too big, needs to be chuncked.
                 * Requires more than 1 fulfilment items */
                int mod = lastSpanDays % queryDateInterval;
                int chunkSize = (int)Math.Floor((double)lastSpanDays / queryDateInterval);
                int chunks = mod != 0 ? ((queryDateInterval - mod) / lastSpanDays) + 1 : queryDateInterval / lastSpanDays;

                for (int i = 0; i < chunks; i++)
                {
                    var toDate = i == 0 ? nextToDate : result[i].Item1.AddDays(-1);
                    var fromDate = toDate.AddDays(-1 * chunkSize);
                    result.Add(new Tuple<DateTime, DateTime>(fromDate, toDate));
                }

                if (mod != 0)
                {
                    var lastToDate = result.Last().Item1.AddDays(-1);
                    var lastFromDate = lastToDate.AddDays(-1 * mod);
                    result.Add(new Tuple<DateTime, DateTime>(lastFromDate, lastToDate));
                }
            }

            return result;
        }

        private FulfilmentItemEntity CreateNewFulfilmentItem(JobItemEntity jobItem,
            FulfilmentEntity lastFulfilment,
            Tuple<DateTime, DateTime> requestDateInterval,
            string baseUrl)
        {
            FulfilmentItemEntity fulfilmentItem = new FulfilmentItemEntity()
            {
                FulfilmentId = lastFulfilment.FulfilmentId, //PK
                ItemUId = Guid.NewGuid(), //RK
                JobItemId = jobItem.JobItemId,
                QuerySubjectCode = jobItem.QuerySubjectCode,
                QuerySubjectGroup = jobItem.QuerySubjectGroup,
                ItemsPerRequest = jobItem.ItemsPerRequest
            };

            fulfilmentItem.Url = ConstructRequestUrl(requestDateInterval, baseUrl, fulfilmentItem);

            return fulfilmentItem;
        }

        private static string ConstructRequestUrl(Tuple<DateTime, DateTime> requestDateInterval, string baseUrl, FulfilmentItemEntity fulfilmentItem)
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

            return UrlMaker.RequestUrlForItemsBetweenDates(urlParams);
        }

        private FulfilmentEntity CreateNewFulfilment(JobEntity job, FulfilmentEntity lastFulfilment)
        {
            return _fulfilmentRepository.SaveFulfilment(new FulfilmentEntity()
            {
                JobId = job.JobId,
                FulfilmentId = Guid.NewGuid(),
                Type = job.Type,
                JobStartedDate = DateTime.UtcNow
            });
        }

        private void ServiceSetup()
        {
            //Seed Subjects if empty
            if (_subjectRepo.GetAll().Result.Count() == 0)
            {
                _logger.LogInformation("Seeding Subject Table.");
                _subjectRepo.Seed();
            }
        }
    }
}
