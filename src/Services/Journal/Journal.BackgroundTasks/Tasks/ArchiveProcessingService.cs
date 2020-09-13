using EventBus.Abstractions;
using Journal.BackgroundTasks.Events;
using Journal.BackgroundTasks.Services;
using Journal.BackgroundTasks.Types;
using Journal.Domain.AggregatesModel.ArticleAggregate;
using Journal.Domain.AggregatesModel.JobAggregate;
using Journal.Infrastructure;
using Journal.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Journal.BackgroundTasks.Tasks
{
    public class ArchiveJournalProcessingService : BackgroundService
    {
        private readonly ILogger<ArchiveJournalProcessingService> _logger;
        private readonly IEventBus _eventBus;
        private readonly ExtractGrpcService _extractApiService;
        private readonly ITransformService _transformService;
        private readonly IFulfillmentRepository _fulfillmentRepository;
        private readonly JournalContext _journalContext;
        private readonly JournalBackgroundTasksConfiguration _config;

        public string AppName { get; set; } = typeof(ArchiveJournalProcessingService).Name;

        public ArchiveJournalProcessingService(ILogger<ArchiveJournalProcessingService> logger,
            IOptions<JournalBackgroundTasksConfiguration> config,
            IEventBus eventBus,
            ExtractGrpcService extractApiService,
            ITransformService transformService,
            IFulfillmentRepository fulfillmentRepository,
            JournalContext journalContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventBus = eventBus;
            _extractApiService = extractApiService;
            _transformService = transformService;
            _fulfillmentRepository = fulfillmentRepository;
            _journalContext = journalContext;
            _config = config?.Value ?? throw new ArgumentException("IOptions<JournalBackgroundTasksConfiguration> not found");
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await DoWork(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogError($"{AppName} - Operation Canceled Exception Occured");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"{AppName} - An Unhandled exception was thrown");
            }
            finally
            {
                await base.StopAsync(stoppingToken);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug($"{AppName} Background Worker is stoppping.");
            return base.
                StopAsync(cancellationToken);
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() => _logger.LogDebug($"#1 {AppName} Background worker is Stopping."));

            if (!_config.ArchiveModeIsActive)
            {
                _logger.LogInformation($"Stopping {AppName} because current Archive Extraction Mode is NOT Active.");

                await StopAsync(stoppingToken);
            }
            else
            {
                _logger.LogInformation($"{AppName} - Background Worker has started doing work - {DateTime.UtcNow}");

                while (!stoppingToken.IsCancellationRequested)
                {
                    var (journalId, extractionFulfillmentId) = await RunArchiveJournalProcessing(stoppingToken);

                    if (journalId != 0)
                    {
                        var journalProcessSucceededIntegrationEvent = new JournalProcessSucceededIntegrationEvent(journalId);
                        journalProcessSucceededIntegrationEvent.JournalType = ProcessTypeEnum.Archive;

                        _logger.LogInformation("----- Publishing Integration Event: {IntegrationEventId} from {AppName} = ({@IntegrationEvent})",
                                                journalProcessSucceededIntegrationEvent.JournalId,
                                                Program.AppName, journalProcessSucceededIntegrationEvent);

                        _eventBus.Publish(journalProcessSucceededIntegrationEvent);
                    }
                    else if (!string.IsNullOrEmpty(extractionFulfillmentId))
                    {
                        var journalProcessFailedIntegrationEvent = new JournalProcessFailedIntegrationEvent(extractionFulfillmentId);
                        journalProcessFailedIntegrationEvent.JournalType = ProcessTypeEnum.Archive;

                        _logger.LogError("----- Publishing Integration Event: {IntegrationEventId} from {AppName} = ({@IntegrationEvent})",
                                               journalProcessFailedIntegrationEvent.ExtractionFulfillmentId,
                                               Program.AppName, journalProcessFailedIntegrationEvent);

                        _eventBus.Publish(journalProcessFailedIntegrationEvent);
                    }

                    _logger.LogInformation($"Waiting for [{_config.PostProcessingWaitTimeInMilliSeconds / 1000}] seconds ....");
                    await Task.Delay(_config.PostProcessingWaitTimeInMilliSeconds, stoppingToken);
                }
            }
        }

        private async Task<(int, string)> RunArchiveJournalProcessing(CancellationToken stoppingToken)
        {
            // 1. Query fulfilment table to find the earliest from the set of unprocessed archive extractions
            var (nextFound, nextFulfillment) = _fulfillmentRepository.FindNextUnprocessedFulfillment(ProcessTypeEnum.Archive);

            if (!nextFound)
            {
                _logger.LogInformation($"Found No fulfillment records to process at [{DateTime.UtcNow}]");
                // await StopAsync(stoppingToken);
                return await Task.FromResult((0, string.Empty));
            }
            else
            {                
                //Query Extract Api for Publications
                PublicationResponseDto responseDto = new PublicationResponseDto();
                responseDto = await _extractApiService.GetExtractedPublications(nextFulfillment.ExtractionFulfillmentId);                
                
                nextFulfillment.SetJobAsStarted();
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                // Invoke transform service to convert to articles
                var (success, allArticles) = _transformService.MapToDomain(responseDto.PublicationItems, nextFulfillment.Id);

                if (!success)
                {
                    nextFulfillment.SetJobAsFailed();
                    stopwatch.Stop();
                    nextFulfillment.SetProcessingTime(stopwatch.ElapsedMilliseconds);
                    _logger.LogError($"Fulfillment Id {nextFulfillment.Id} - Mapping Publications to Articles failed");
                    await StopAsync(stoppingToken);
                    return await Task.FromResult((0, nextFulfillment.ExtractionFulfillmentId));
                }
                else
                {
                    int updatedCount = 0;
                    int insertedCount = 0;
                    string[] existingArxivIds = Array.Empty<string>();

                    // Fetch Articles for the subset that already exist by in Database (canonical) arxiv id
                    var ids = allArticles?.Select(x => x.ArxivId)?.ToArray();

                    //Eager Load all existing articles in DB
                    List<Article> existingArticles = GetExistingArticlesByArxivId(ids);

                    if (existingArticles != null && existingArticles.Count > 0)
                    {
                        //Get List of existing arxiv Ids
                        existingArxivIds = existingArticles?.Select(x => x.ArxivId)?.ToArray();

                        // Perform 'update' operations on 'existing' with 'new version', in the dbContext
                        foreach (var existing in existingArticles)
                        {
                            var newArticleVersion = allArticles.Where(x => x.ArxivId == existing.ArxivId).FirstOrDefault();
                            existing.Update(newArticleVersion);
                            _journalContext.Articles.Attach(existing);
                            updatedCount++;
                        }
                    }

                    //Select articles to be inserted
                    var tobeInsertedArticles = allArticles.Where(x => existingArxivIds.Contains(x.ArxivId) == false)?.ToList();

                    // Perform 'add/insert' or 'update' operations on this 'new' subset
                    foreach (var inserted in tobeInsertedArticles ?? new List<Article>())
                    {
                        // Query the 'new' articles sub set for duplicates i.e. different version of same arxivid
                        var existing = _journalContext.Articles.Where(a => a.ArxivId == inserted.ArxivId).FirstOrDefault();

                        if (existing != default(Article))
                        {
                            ///The scenario where an article to be 'inserted' has an existing version already inserted
                            ///in the dbConetxt witihn this processing session. The correct action is to call domain method 'Update'.
                            existing.Update(inserted);
                            _journalContext.Articles.Attach(existing);
                            updatedCount++;
                        }
                        else
                        {
                            _journalContext.Articles.Attach(inserted);
                            insertedCount++;
                        }
                    }

                    await _journalContext.SaveChangesAsync(CancellationToken.None);

                    _logger.LogInformation($"Saving Articles to Database from Fulfillment : id {nextFulfillment.Id}");

                    stopwatch.Stop();

                    // Update/Set journal fulfillment values (set as processed)
                    nextFulfillment.SetProcessingTime(stopwatch.ElapsedMilliseconds);
                    nextFulfillment.UpdateCounts(allArticles.Count, insertedCount, updatedCount,
                                                tobeInsertedArticles.Count + existingArticles.Count);
                    nextFulfillment.SetJobAsCompeleted();

                    //Return journal fulfillment id
                    int journalId = _fulfillmentRepository.Save(nextFulfillment).Id;

                    return (journalId, nextFulfillment.ExtractionFulfillmentId);
                }
            }
        }

        private List<Article> GetExistingArticlesByArxivId(string[] arxivIds)
        {
            if (arxivIds == null | arxivIds.Length == 0)
                return new List<Article>();

            return _journalContext.Articles
                                .Include(x => x.AuthorArticles)
                                .Include(x => x.PaperVersions)
                                .Include(x => x.CategoryArticles)
                                .Where(x => arxivIds.Contains(x.ArxivId))
                                ?.OrderBy(x => x.ArxivId)
                                ?.ToList() ?? new List<Article>();
        }
    }
}
