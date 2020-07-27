﻿using arx.Extract.BackgroundTasks.Core;
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
using System.Threading;
using System.Threading.Tasks;

namespace arx.Extract.BackgroundTasks.Tasks
{
    public class ArchiveExtractionService : BackgroundService
    {
        private readonly BackgroundTaskSettings _settings;
        private readonly IMapper _mapper;
        private readonly IEventBus _eventBus;
        private readonly ILogger<ArchiveExtractionService> _logger;
        private readonly ISubjectRepository _subjectRepo;
        private readonly IJobRepository _jobRepository;
        private readonly IJobItemRepository _jobItemRepository;
        private readonly IFulfillmentRepository _fulfillmentRepository;
        private readonly IFulfillmentItemRepository _fulfillmentItemRepository;
        private readonly IPublicationRepository _publicationRepository;
        private readonly IArchiveFetch _archiveFetch;
        private readonly ITransformService _transformService;

        public ArchiveExtractionService(IOptions<BackgroundTaskSettings> settings,
            IMapper mapper,
            IEventBus eventBus,
            ILogger<ArchiveExtractionService> logger,
            ISubjectRepository subjectRepo,
            IJobRepository jobRepository,
            IJobItemRepository jobItemRepository,
            IFulfillmentRepository fulfillmentRepository,
            IFulfillmentItemRepository fulfillmentItemRepository,
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
            _fulfillmentRepository = fulfillmentRepository;
            _fulfillmentItemRepository = fulfillmentItemRepository;
            _publicationRepository = publicationRepository;
            _archiveFetch = archiveFetch;
            _transformService = transformService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug($"{this.GetType().Name} Background Task is starting.");
            try
            {
                await DoWork(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogError($"{this.GetType().Name} - Operation Canceled Exception Occured");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex,$"{this.GetType().Name} - An Unhandled exception was thrown");
            }
            finally
            {
                await base.StopAsync(stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {  
            _logger.LogDebug($"{this.GetType().Name} Background Task is stopping.");
            await base.StopAsync(cancellationToken);            
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            ServiceSeedSetup();

            stoppingToken.Register(() => _logger.LogDebug($"#1 {this.GetType().Name} background task is stopping."));

            if (!_settings.ArchiveModeIsActive)
            {
                _logger.LogInformation($"Stopping {this.GetType().Name} because current Archive Extraction Mode is NOT Active.");

                await StopAsync(stoppingToken);
            }
            else
            {
                _logger.LogDebug($"{this.GetType().Name} background task is doing background work. [{DateTime.UtcNow}]");

                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation($"Waiting For [{_settings.PostFetchWaitTime / 1000}] seconds before starting next extraction cycle...");

                    string newFulfillmentId = await RunArchiveExtraction(stoppingToken);

                    var extractionCompletedEvent = new ExtractionCompletedIntegrationEvent(newFulfillmentId);

                    _logger.LogInformation("----- Publishing Integration Event: {IntegrationEventId} from {AppName} = ({@IntegrationEvent})",
                                            extractionCompletedEvent.ExtractionId, Program.AppName, extractionCompletedEvent);

                    _eventBus.Publish(extractionCompletedEvent);

                    await Task.Delay(_settings.PostFetchWaitTime, stoppingToken);
                }
            }
        }

        private async Task<string> RunArchiveExtraction(CancellationToken stoppingToken)
        {
            _logger.LogDebug("Reading Metadata to determine archive task parameters.");

            JobEntity job = _jobRepository.GetJob(ExtractTypeEnum.Archive, _settings.ArchiveJobName);

            List<JobItemEntity> jobItems = _jobItemRepository.GetJobItems(job.UniqueName);

            if (jobItems == null || jobItems.Count == 0)
            {
                //TODO:Retry, fail logic
            }

            FulfillmentEntity lastFulfillment = _fulfillmentRepository.GetLastSuccessfulFulfillment(job.UniqueName).Result;
                       
            int minQueryDateInterval = (int)Math.Floor(jobItems.Average(x => x.QueryDateInterval));

            FulfillmentEntity newFulfillment = ExtractUtil.MakeNewFulfillment(job, lastFulfillment, minQueryDateInterval);

             newFulfillment = _fulfillmentRepository.SaveFulfillment(newFulfillment).Result;

            if (newFulfillment == null)
            {
                _logger.LogCritical("Error persisting New Fulfilment to Storage - [{0}]-[{1}] - Query From [{2}] To [{3}]",
                        newFulfillment.JobName, newFulfillment.FulfillmentId, newFulfillment.QueryFromDate.ToString("dd MMMM yyyy")
                        , newFulfillment.QueryToDate.ToString("dd MMMM yyyy"));
                await StopAsync(stoppingToken);
            }
            else
            {
                _logger.LogInformation("New Fulfillment [{0}]-[{1}] - Query From [{2}] To [{3}] - Started @ {4}",
                                    newFulfillment.JobName, newFulfillment.FulfillmentId, newFulfillment.QueryFromDate.ToString("dd MMMM yyyy")
                                    , newFulfillment.QueryToDate.ToString("dd MMMM yyyy"), newFulfillment.JobStartedDate);

                List<FulfillmentItemEntity> newFulfillmentItems = new List<FulfillmentItemEntity>();

                foreach (var jobItem in jobItems)
                {
                    List<ExtractQueryDates> requestDateIntervals = ExtractUtil.GetRequestChunkedArchiveDates(lastFulfillment, jobItem.QueryDateInterval);

                    foreach (var interval in requestDateIntervals)
                    {
                        newFulfillmentItems.Add(ExtractUtil.MakeNewFulfillmentItem(jobItem, interval, job.QueryBaseUrl, newFulfillment.FulfillmentId));
                    }
                }

                List<FulfillmentItemEntity> fulfillmentItems = _fulfillmentItemRepository.SaveFulfillmentItems(newFulfillmentItems);

                if (fulfillmentItems == null || fulfillmentItems.Count == 0)
                {
                    _logger.LogCritical($"Error persisting [{fulfillmentItems.Count}] New Fulfillment Items from Fulfillment to Storage {newFulfillment.FulfillmentId} - @{DateTime.UtcNow}");
                    await StopAsync(stoppingToken);
                }
                else
                {
                    _logger.LogInformation($"Created [{fulfillmentItems.Count}] New Fulfillment Items from Fulfillment {newFulfillment.FulfillmentId} - @{DateTime.UtcNow}");

                    Stopwatch stopwatch = new Stopwatch();

                    //Run Http Request, transform, persist operations per Fulfillment Item Entry
                    foreach (var fulfillmentItem in fulfillmentItems)
                    {
                        List<ArxivItem> allResults = new List<ArxivItem>();

                        fulfillmentItem.JobItemStartDate = DateTime.UtcNow;

                        //Make initial http request to external website/API          
                        stopwatch.Start();
                        var (initialResponse, initialItems) = _archiveFetch.GetArxivItems(fulfillmentItem.Url).Result;
                        stopwatch.Stop();

                        //Save http request time elapsed.
                        fulfillmentItem.HttpRequestIsSuccess = initialResponse.IsSuccessStatusCode;
                        fulfillmentItem.FetchTimeSpan = stopwatch.ElapsedMilliseconds;
                        stopwatch.Reset();

                        fulfillmentItem.HttpRequestCount++;

                        //Add response to results list
                        allResults.Add(initialItems);

                        int totalAvailable = initialItems.totalResults;
                        int fetched = initialItems.itemsPerPage;
                        string initialUrl = fulfillmentItem.Url;

                        fulfillmentItem.TotalResults = fetched < totalAvailable ? fetched : totalAvailable;
                        fulfillmentItem.ResultSizePerHttpRequest = fetched;

                        if (fetched < totalAvailable)
                        {
                            _logger.LogInformation($"FulfillmentItem {fulfillmentItem.ItemUId} - Only fetched[{fetched}] out of [{totalAvailable}]. Making further Paged Http Requests...");

                            //Calculate the number of requests required to fetch all items for the initial query
                            int requests = ExtractUtil.CalculatePagedRequestCount(totalAvailable, fetched);

                            //Get delay value from settings/Environment
                            int delay = _settings.ArxivApiPagingRequestDelay;

                            //Record delay value
                            fulfillmentItem.DelayBetweenHttpRequests = delay * 1000;

                            for (int i = 0; i < requests; i++)
                            {
                                //Apply delay, as per the request by Arxiv.org api access policy.
                                await Task.Delay(fulfillmentItem.DelayBetweenHttpRequests);

                                //Calculate current start index value
                                int currentStartIndex = (i + 1) * fetched + 1;

                                //Add next start index to request url
                                string pagedUrl = $"{initialUrl}&start={currentStartIndex}";

                                _logger.LogInformation($"FulfillmentItem {fulfillmentItem.ItemUId} - Making paged Http request no [{i + 1}]");
                                stopwatch.Start();
                                var (pagedResponse, pagedItems) = _archiveFetch.GetArxivItems(pagedUrl).Result;
                                stopwatch.Stop();

                                //Add meta data
                                fulfillmentItem.FetchTimeSpan += stopwatch.ElapsedMilliseconds;
                                stopwatch.Reset();
                                fulfillmentItem.HttpRequestCount++;
                                fulfillmentItem.HttpRequestIsSuccess = pagedResponse.IsSuccessStatusCode;

                                //Add reponse to result list
                                allResults.Add(pagedItems);

                                fulfillmentItem.TotalResults += pagedItems?.EntryList?.Count ?? 0;
                            }
                        }

                        //Re-Set FetchTimeSpan as time taken handling Http requests, if there have been Paged/more than one requests.
                        if (fulfillmentItem.DelayBetweenHttpRequests > 0)
                        {
                            fulfillmentItem.FetchTimeSpan = (DateTime.UtcNow - fulfillmentItem.JobItemStartDate).TotalMilliseconds;
                        }

                        newFulfillment.TotalCount += fulfillmentItem.TotalResults;

                        //Tranform and Persist to Storage
                        if (fulfillmentItem.TotalResults < 1)
                        {
                            _logger.LogInformation($"FulfillmentItem {fulfillmentItem.ItemUId} - returned 0 items.");
                        }
                        else
                        {
                            List<Entry> allArxivEntries = new List<Entry>();

                            //Accumulate all Arxiv Entry values
                            allResults?.ForEach(x => allArxivEntries.AddRange(x.EntryList));

                            if (allArxivEntries.Count == 0)
                            {
                                fulfillmentItem.DataExtractionIsSuccess = true;
                            }
                            else
                            {
                                //Perform transformations
                                var publications = _transformService.TransformArxivEntriesToPublications(allArxivEntries);

                                //Set type transformaiton success
                                fulfillmentItem.DataExtractionIsSuccess = (publications.Count == allArxivEntries.Count);

                                //Set fulfillment Ids
                                var entityList = _mapper.Map<List<PublicationItemEntity>>(publications);
                                entityList?.ForEach(e =>
                                {
                                    e.PartitionKey = newFulfillment.FulfillmentId.ToString();
                                    e.FulfillmentId = newFulfillment.FulfillmentId.ToString();
                                    e.FulFillmentItemId = fulfillmentItem.ItemUId.ToString();
                                });

                                //Persist publications to Storage - Batch insert
                                int saved = await _publicationRepository.BatchSavePublications(entityList);

                                if (saved == 0)
                                {
                                    _logger.LogError($"FulfillmentItem {fulfillmentItem.ItemUId} - persisted 0 publications to Storage");
                                }
                            }
                        }

                        //Record process completion Time, interval               
                        fulfillmentItem.JobItemCompletedDate = DateTime.UtcNow;
                        fulfillmentItem.TotalProcessingInMilliseconds =
                            (fulfillmentItem.JobItemCompletedDate - fulfillmentItem.JobItemStartDate).TotalMilliseconds;

                        //Save to fulfillment Item to database
                        var savedItem = _fulfillmentItemRepository.SaveFulfillmentItem(fulfillmentItem);

                        //Log fulfillment Item summary
                        string activeCode = string.IsNullOrEmpty(fulfillmentItem.QuerySubjectCode) ? fulfillmentItem.QuerySubjectGroup : fulfillmentItem.QuerySubjectCode;
                        string logFulfillmentItem = $"FulfillmentItem [{fulfillmentItem.ItemUId}] - Subject Query [{activeCode}] - Started @{fulfillmentItem.JobItemStartDate} - Completed @{fulfillmentItem.JobItemCompletedDate} - Fetched ={fulfillmentItem.TotalResults}";

                        if (savedItem != null)
                            _logger.LogInformation(logFulfillmentItem);
                        else
                            _logger.LogError($"Error Saving {logFulfillmentItem}");
                    }

                    //Save new Fulilment record values
                    newFulfillment.PartialSuccess = fulfillmentItems.Any(x => x.HttpRequestIsSuccess == true)
                       && fulfillmentItems.Any(x => x.DataExtractionIsSuccess = true);

                    newFulfillment.CompleteSuccess = fulfillmentItems.All(x => x.HttpRequestIsSuccess == true)
                        && fulfillmentItems.All(x => x.DataExtractionIsSuccess = true);

                    newFulfillment.JobCompletedDate = DateTime.UtcNow;
                    newFulfillment.ProcessingTimeInSeconds = (newFulfillment.JobCompletedDate - newFulfillment.JobStartedDate).TotalSeconds;

                    //Persist to Storage
                    var savedNew = await _fulfillmentRepository.SaveFulfillment(newFulfillment);

                    if (savedNew != null)
                    {
                        _logger.LogInformation($"Fulfillment {newFulfillment.JobName} -[{newFulfillment.FulfillmentId}] - Completed @{newFulfillment.JobCompletedDate} - Total Count = {newFulfillment.TotalCount} - From [{ newFulfillment.QueryFromDate}] To [{ newFulfillment.QueryToDate}]");
                    }
                    else
                    {
                        _logger.LogInformation($@"Error Saving - Fulfillment {newFulfillment.JobName} -[{newFulfillment.FulfillmentId}] - Completed @{newFulfillment.JobCompletedDate} - Total Count = {newFulfillment.TotalCount} - From [{ newFulfillment.QueryFromDate}] To [{ newFulfillment.QueryToDate}]");
                    }
                }
            }

            return newFulfillment.FulfillmentId.ToString();
        }
        
        /// <summary>
        /// Adds Seed data to Storage, if Table's are empty.
        /// Used to Seed job template data for services.
        /// </summary>
        private void ServiceSeedSetup()
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