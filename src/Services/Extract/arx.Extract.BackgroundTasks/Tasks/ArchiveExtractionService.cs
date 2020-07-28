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
using System.Threading;
using System.Threading.Tasks;

namespace arx.Extract.BackgroundTasks.Tasks
{
    public class ArchiveExtractionService : BackgroundService
    {
        private readonly BackgroundTaskSettings _settings;
        private readonly IEventBus _eventBus;
        private readonly ILogger<ArchiveExtractionService> _logger;
        private readonly IExtractService _extractService;
        private readonly ISubjectRepository _subjectRepo;
        private readonly IJobRepository _jobRepository;
        private readonly IJobItemRepository _jobItemRepository;
        private readonly IFulfillmentRepository _fulfillmentRepository;
        private readonly IFulfillmentItemRepository _fulfillmentItemRepository;
        private readonly IPublicationRepository _publicationRepository;
        private readonly IArchiveFetch _archiveFetch;
        private readonly ITransformService _transformService;

        public ArchiveExtractionService(IOptions<BackgroundTaskSettings> settings,
            IEventBus eventBus,
            ILogger<ArchiveExtractionService> logger,
            IExtractService extractService,
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
            _eventBus = eventBus;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _extractService = extractService;
            _subjectRepo = subjectRepo;
            _jobRepository = jobRepository;
            _jobItemRepository = jobItemRepository;
            //_fulfillmentRepository = fulfillmentRepository;
            //_fulfillmentItemRepository = fulfillmentItemRepository;
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
                _logger.LogCritical(ex, $"{this.GetType().Name} - An Unhandled exception was thrown");
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
                    string newFulfillmentId = await RunArchiveExtraction(stoppingToken);

                    if (!string.IsNullOrEmpty(newFulfillmentId))
                    {
                        var extractionCompletedEvent = new ExtractionCompletedIntegrationEvent(newFulfillmentId);

                        _logger.LogInformation("----- Publishing Integration Event: {IntegrationEventId} from {AppName} = ({@IntegrationEvent})",
                                                extractionCompletedEvent.ExtractionId, Program.AppName, extractionCompletedEvent);

                        _eventBus.Publish(extractionCompletedEvent);
                    }

                    _logger.LogInformation($"Waiting For [{_settings.PostFetchWaitTime / 1000}] seconds before starting next extraction cycle...");
                    await Task.Delay(_settings.PostFetchWaitTime, stoppingToken);
                }
            }
        }

        private async Task<string> RunArchiveExtraction(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Reading Archive Task Metadata from Storage...");

            var (jobFetchSuccess, job, jobItems) = _extractService.GetArchiveJob();
            
            if (!jobFetchSuccess)
            {
                _logger.LogCritical("Error fetching Job Archive metadata from Storage");
                await StopAsync(stoppingToken);
            }
            else
            {
                var (_, lastFulfillment) = _extractService.GetLastSuccessfulArchiveFulfillment(job.UniqueName);

                var (createSuccess, newFulfillment, newFulfillmentItems) 
                    = _extractService.CreateArchiveFulfillmentSaga(job, jobItems, lastFulfillment);

                if (!createSuccess)
                {
                    await StopAsync(stoppingToken);
                }
                else
                {
                    _logger.LogInformation($"Created [{newFulfillmentItems.Count}] New Fulfillment Items from Fulfillment {newFulfillment.FulfillmentId} - @{DateTime.UtcNow}");
                    Stopwatch stopwatch = new Stopwatch();

                    //Run Http Request, transform, persist operations per Fulfillment Item Entry
                    foreach (var fulfillmentItem in newFulfillmentItems)
                    {
                        List<ArxivItem> allResults = new List<ArxivItem>();

                        fulfillmentItem.JobItemStartDate = DateTime.UtcNow;

                        //Make initial http request to external website/API          
                        stopwatch.Start();
                        var (initialResponse, initiaResultItems) = _archiveFetch.GetArxivItems(fulfillmentItem.Url).Result;
                        stopwatch.Stop();

                        //Save http request time elapsed.
                        fulfillmentItem.HttpRequestIsSuccess = initialResponse.IsSuccessStatusCode;
                        fulfillmentItem.FetchTimeSpan = stopwatch.ElapsedMilliseconds;
                        stopwatch.Reset();

                        fulfillmentItem.HttpRequestCount++;

                        //Add response to results list
                        allResults.Add(initiaResultItems);

                        int totalAvailable = initiaResultItems.totalResults;
                        int fetched = initiaResultItems.itemsPerPage;
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
                                var (success, count, publications) = _transformService.TransformArxivEntriesToPublications(allArxivEntries);

                                //Set type transformaiton success
                                fulfillmentItem.DataExtractionIsSuccess = success;

                                //Log errors in transformation process
                                if (!success)
                                {
                                    _logger.LogError("Fulfillment Item [{0}] - Error Transforming ArxivEntries To Publicationitems. {1}/{2} were successful.",
                                                        fulfillmentItem.ItemUId, count, allArxivEntries);
                                }
                                else
                                {
                                    var (mapIsSuccess, entityList) = _transformService.TransformPublicationItemsToEntity
                                                                                        (newFulfillment.FulfillmentId.ToString(),
                                                                                        fulfillmentItem.ItemUId.ToString(),
                                                                                        publications);

                                    if (!mapIsSuccess)
                                    {
                                        _logger.LogCritical("FulfillmentItem {0} - Error Transforming PublicationItems To PublicationItemEntity", fulfillmentItem.ItemUId);
                                    }
                                    else
                                    {
                                        //Persist publications to Storage - Batch insert
                                        if (await _publicationRepository.BatchSavePublications(entityList) == 0)
                                        {
                                            _logger.LogCritical("FulfillmentItem {0} - FAILED to persist publications to Storage", fulfillmentItem.ItemUId);
                                        }
                                    }
                                }
                            }
                        }

                        //Record process completion Time, interval               
                        fulfillmentItem.JobItemCompletedDate = DateTime.UtcNow;
                        fulfillmentItem.TotalProcessingInMilliseconds =
                            (fulfillmentItem.JobItemCompletedDate - fulfillmentItem.JobItemStartDate).TotalMilliseconds;

                        //Persist fulfillment Item to Storage
                        _extractService.UpdateFulfilmentItem(fulfillmentItem);
                    }

                    //Set Fulilment record values
                    newFulfillment.JobCompletedDate = DateTime.UtcNow;
                    newFulfillment.PartialSuccess = newFulfillmentItems.Any(x => x.HttpRequestIsSuccess) && newFulfillmentItems.Any(x => x.DataExtractionIsSuccess);
                    newFulfillment.CompleteSuccess = newFulfillmentItems.All(x => x.HttpRequestIsSuccess) && newFulfillmentItems.All(x => x.DataExtractionIsSuccess);
                    newFulfillment.ProcessingTimeInSeconds = (newFulfillment.JobCompletedDate - newFulfillment.JobStartedDate).TotalSeconds;

                    //Persist to Storage
                   await  _extractService.UpdateFulfilment(newFulfillment);
                }
                return newFulfillment.FulfillmentId.ToString();
            }

            return string.Empty;
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
