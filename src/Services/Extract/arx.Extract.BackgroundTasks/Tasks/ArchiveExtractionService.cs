using arx.Extract.BackgroundTasks.Core;
using arx.Extract.BackgroundTasks.Events;
using arx.Extract.Data.Repository;
using arx.Extract.Lib;
using EventBus.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
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
        private readonly BackgroundTasksConfiguration _config;
        private readonly IEventBus _eventBus;
        private readonly IExtractService _extractService;
        private readonly ISubjectRepository _subjectRepo;
        private readonly IJobRepository _jobRepository;
        private readonly IJobItemRepository _jobItemRepository;
        private readonly IPublicationRepository _publicationRepository;
        private readonly IArchiveFetch _archiveFetch;
        private readonly ITransformService _transformService;

        public ArchiveExtractionService(IOptions<BackgroundTasksConfiguration> config,
            IEventBus eventBus,
            IExtractService extractService,
            ISubjectRepository subjectRepo,
            IJobRepository jobRepository,
            IJobItemRepository jobItemRepository,
            IPublicationRepository publicationRepository,
            IArchiveFetch archiveFetch,
            ITransformService transformService)
        {
            _config = config?.Value ?? throw new ArgumentException("IOptions<BackgroundTasksConfiguration> is not configured or null");
            _eventBus = eventBus;
            _extractService = extractService;
            _subjectRepo = subjectRepo;
            _jobRepository = jobRepository;
            _jobItemRepository = jobItemRepository;
            _publicationRepository = publicationRepository;
            _archiveFetch = archiveFetch;
            _transformService = transformService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Debug($"{this.GetType().Name} Background Task is starting.");
            try
            {
                await DoWork(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                Log.Error($"{this.GetType().Name} - Operation Canceled Exception Occured");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, $"{this.GetType().Name} - An Unhandled exception was thrown");
            }
            finally
            {
                await base.StopAsync(stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            Log.Debug($"{this.GetType().Name} Background Task is stopping.");
            await base.StopAsync(cancellationToken);
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            ServiceSeedSetup();

            stoppingToken.Register(() => Log.Debug($"#1 {this.GetType().Name} background task is stopping."));

            if (!_config.ArchiveModeIsActive)
            {
                Log.Information($"Stopping {this.GetType().Name} because current Archive Extraction Mode is NOT Active.");

                await StopAsync(stoppingToken);
            }
            else
            {
                Log.Debug($"{this.GetType().Name} background task is doing background work. [{DateTime.UtcNow}]");

                while (!stoppingToken.IsCancellationRequested)
                {
                    string newFulfillmentId = await RunArchiveExtraction(stoppingToken);

                    if (!string.IsNullOrEmpty(newFulfillmentId))
                    {
                        var archiveExtractionSucceededIntegrationEvent = new ArchiveExtractionSucceededIntegrationEvent(newFulfillmentId);

                        Log.Information("----- Publishing Integration Event: {IntegrationEventId} from {AppName} = ({@IntegrationEvent})",
                                                archiveExtractionSucceededIntegrationEvent.ExtractionFulfillmentId,
                                                Program.AppName,
                                                archiveExtractionSucceededIntegrationEvent);

                        _eventBus.Publish(archiveExtractionSucceededIntegrationEvent);
                    }

                    Log.Information($"Waiting For [{_config.PostFetchWaitTime / 1000}] seconds before starting next extraction cycle...");
                    await Task.Delay(_config.PostFetchWaitTime, stoppingToken);
                }
            }
        }

        private async Task<string> RunArchiveExtraction(CancellationToken stoppingToken)
        {
            Log.Information("Reading Archive Task Metadata from Storage...");

            var (jobFetchSuccess, job, jobItems) = _extractService.GetArchiveJob();

            if (!jobFetchSuccess)
            {
                Log.Error("Error fetching Job Archive metadata from Storage");
                await StopAsync(stoppingToken);
            }
            else
            {
                var (isFound, isFirstFulfillment, lastFulfillment) = _extractService.GetLastSuccessfulArchiveFulfillment(job.UniqueName);

                if (!isFirstFulfillment && !isFound)
                {
                    // 
                    Log.Information("Did not find a successfully compeleted Archive Fulfillment for jobName {0}", job.UniqueName);                    
                }
                else
                {
                    var (createSuccess, newFulfillment, newFulfillmentItems)
                        = _extractService.CreateArchiveFulfillmentSaga(job, jobItems, lastFulfillment, isFirstFulfillment);

                    if (!createSuccess)
                    {
                        await StopAsync(stoppingToken);
                    }
                    else
                    {
                        Log.Information($"Created [{newFulfillmentItems.Count}] New Fulfillment Items from Fulfillment {newFulfillment.FulfillmentId} - @{DateTime.UtcNow}");
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
                                Log.Information($"FulfillmentItem {fulfillmentItem.ItemUId} - Only fetched[{fetched}] out of [{totalAvailable}]. Making further Paged Http Requests...");

                                //Calculate the number of requests required to fetch all items for the initial query
                                int requests = ExtractUtil.CalculatePagedRequestCount(totalAvailable, fetched);

                                //Get delay value from settings/Environment
                                int delay = _config.ArxivApiPagingRequestDelay;

                                //Record delay value
                                fulfillmentItem.DelayBetweenHttpRequests = delay * 1000;

                                for (int i = 0; i < requests; i++)
                                {
                                    //Apply delay, as per the request by Arxiv.org api access policy.
                                    await Task.Delay(fulfillmentItem.DelayBetweenHttpRequests, CancellationToken.None);

                                    //Calculate current start index value
                                    int currentStartIndex = (i + 1) * fetched + 1;

                                    //Add next start index to request url
                                    string pagedUrl = $"{initialUrl}&start={currentStartIndex}";

                                    Log.Information($"FulfillmentItem {fulfillmentItem.ItemUId} - Making paged Http request no [{i + 1}]");
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
                                Log.Information($"FulfillmentItem {fulfillmentItem.ItemUId} - returned 0 items.");
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
                                    var (success, transformedCount, publications) = _transformService.TransformArxivEntriesToPublications(allArxivEntries);

                                    //Set type transformaiton success
                                    fulfillmentItem.DataExtractionIsSuccess = success;

                                    //Log errors in transformation process
                                    if (!success)
                                    {
                                        Log.Error($"Fulfillment Item [{fulfillmentItem.ItemUId}] - Error Transforming ArxivEntries To Publicationitems. Only {transformedCount}/{allArxivEntries.Count} (transformed/fetched) were successful.",
                                                            fulfillmentItem.ItemUId, transformedCount, allArxivEntries);
                                    }

                                    if (transformedCount > 0)
                                    {
                                        var (mapIsSuccess, entityList)
                                            = _transformService.TransformPublicationItemsToEntity(newFulfillment.FulfillmentId.ToString(),
                                                                                                fulfillmentItem.ItemUId.ToString(),
                                                                                                publications);

                                        if (!mapIsSuccess)
                                        {
                                            Log.Error("FulfillmentItem {0} - Error Transforming PublicationItems To PublicationItemEntity", fulfillmentItem.ItemUId);
                                        }
                                        else
                                        {
                                            //Persist publications to Storage - Batch insert
                                            if (await _publicationRepository.BatchSavePublications(entityList) == 0)
                                            {
                                                Log.Error("FulfillmentItem {0} - FAILED to persist publications to Storage", fulfillmentItem.ItemUId);
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
                        await _extractService.UpdateFulfilment(newFulfillment);
                    }
                    return newFulfillment.FulfillmentId.ToString();
                }
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
                Log.Information($"Seeding {_subjectRepo.TableName()} Table.");

                if (_subjectRepo.SeedSubjects())
                    Log.Information($"Successfullly Inserted Seed data to  {_subjectRepo.TableName()} Table.");
                else
                    Log.Debug($"Error inserting Seed data to  {_subjectRepo.TableName()}  Table");
            }

            //Seed Jobs if Empty
            if (_jobRepository.HasSeed() == false)
            {
                Log.Information($"Seeding {_jobRepository.TableName()}  Table.");
                if (_jobRepository.SeedJobs())
                    Log.Information($"Successfullly Inserted Seed data to {_jobRepository.TableName()} Table.");
                else
                    Log.Debug($"Error inserting Seed data to  {_jobRepository.TableName()}  Table");
            }

            //Seed JobItems if Empty
            if (_jobItemRepository.HasSeed() == false)
            {
                Log.Information($"Seeding {_jobItemRepository.TableName()}  Table.");
                if (_jobItemRepository.SeedJobItems())
                    Log.Information($"Successfullly Inserted Seed data to {_jobItemRepository.TableName()} Table.");
                else
                    Log.Debug($"Error inserting Seed data to  {_jobItemRepository.TableName()} Table");
            }
        }
    }
}
