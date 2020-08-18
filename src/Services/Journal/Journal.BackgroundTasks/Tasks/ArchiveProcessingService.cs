using Common.Types;
using EventBus.Abstractions;
using Journal.BackgroundTasks.Core;
using Journal.BackgroundTasks.Events;
using Journal.Domain.AggregatesModel.ArticleAggregate;
using Journal.Domain.AggregatesModel.JobAggregate;
using Journal.Infrastructure;
using Journal.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
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
        private readonly ITransformService _transformService;
        private readonly IFulfillmentRepository _fulfillmentRepository;
        private readonly ArticleContext _articleContext;
        private readonly JournalBackgroundTasksConfiguration _config;

        public string AppName { get; set; } = typeof(ArchiveJournalProcessingService).Name;

        public ArchiveJournalProcessingService(ILogger<ArchiveJournalProcessingService> logger,
            IOptions<JournalBackgroundTasksConfiguration> config,
            IEventBus eventBus,
            ITransformService transformService,
            IFulfillmentRepository fulfillmentRepository,
            ArticleContext articleContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventBus = eventBus;
            _transformService = transformService;
            _fulfillmentRepository = fulfillmentRepository;
            _articleContext = articleContext;
            _config = config?.Value ?? throw new ArgumentException(nameof(config));
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
                    int journalId = await RunArchiveJournalProcessing(stoppingToken);

                    if (journalId != 0)
                    {
                        var journalProcessedIntegrationEvent = new JournalProcessedIntegrationEvent(journalId);
                        journalProcessedIntegrationEvent.JournalType = ProcessTypeEnum.Archive;

                        _logger.LogInformation("----- Publishing Integration Event: {IntegrationEventId} from {AppName} = ({@IntegrationEvent})",
                                                journalProcessedIntegrationEvent.JournalId, Program.AppName, journalProcessedIntegrationEvent);

                        _eventBus.Publish(journalProcessedIntegrationEvent);
                    }

                    _logger.LogInformation($"Waiting for [{_config.PostProcessingWaitTimeInMilliSeconds / 1000}] seconds ....");
                    await Task.Delay(_config.PostProcessingWaitTimeInMilliSeconds, stoppingToken);
                }
            }
        }

        private async Task<int> RunArchiveJournalProcessing(CancellationToken stoppingToken)
        {
            // 1. Query fulfilment table to find unprocessed archive extractions, get oldest if any
            var (fromDate, toDate) = _fulfillmentRepository.FindLatestProcessedQueryDates(ProcessTypeEnum.Archive);
            if (fromDate == DateTime.MinValue || toDate == DateTime.MinValue)
            {
                //First ever entry scenario
                //Ask extraction api for  'newest' archive query from & todate record
            }
            else
            {
                //Ask extraction api for record that 'earlier' to  the above fromDate
            }

            // 2. If non fulfilments, ask extract api next to process by last processed q.from and q.to dates in fulfilment, 
            //      -- get and save task to fulfilment

            string extractionFulfillmentId = ""; //mock value
            DateTime extractionQueryFromDate = new DateTime();//mock value
            DateTime extractionQueryToDate = new DateTime(); //mock value

            var newFulfillment = new Fulfillment(string.Empty, extractionFulfillmentId, ProcessTypeEnum.Archive, extractionQueryFromDate, extractionQueryToDate);

            var fulfillment = _fulfillmentRepository.Save(newFulfillment);

            // using 1 or 2, ask extraction  api for publication data for the extractionId in 1 or 2     
            List<ArxivPublication> arxivPublications = new List<ArxivPublication>();

            newFulfillment.SetJobAsStarted();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // invoke transform service to convert to articles
            var (success, allArticles) = _transformService.MapToDomain(arxivPublications, fulfillment.Id);
            if (!success)
            {
                newFulfillment.SetJobAsFailed();
                Log.Error($"Fulfillment Id {fulfillment.Id} - Mapping Publications to Articles failed");
                await StopAsync(stoppingToken);
                return await Task.FromResult(0);
            }

            int updatedCount = 0;
            int insertedCount = 0;
            // Query Article db for the subset that already exist by (canonical) arxiv id
            var ids = allArticles?.Select(x => x.ArxivId)?.ToList();

            ///Eager Load all articles in DB with matchin ArxivId
            var existingArticles = GetExistingArticlesByArxivId(ids);

            //Get List of ecisting arxiv Ids
            List<string> existingIds = existingArticles?.Select(x => x.ArxivId)?.ToList() ?? new List<string>();

            //Select articles to be updated
            var tobeUpdatedArticles = allArticles.Where(x => existingIds.Contains(x.ArxivId))
                                                ?.OrderBy(x => x.ArxivId)
                                                ?.ToList() ?? new List<Article>();

            // Perform 'update' operations on this subset, in the dbContext
            if (tobeUpdatedArticles.Count > 0)
            {
                updatedCount = tobeUpdatedArticles.Count;
                _articleContext.Articles.AttachRange(tobeUpdatedArticles);
            }

            //Select articles to be inserted
            var tobeInsertedArticles = allArticles.Where(x => existingIds.Contains(x.ArxivId) == false)
                                                ?.OrderBy(x => x.ArxivId)
                                                ?.ToList() ?? new List<Article>();

            // Perform 'add/insert' operations on this 'new' subset
            if (tobeInsertedArticles.Count() > 0)
            {
                foreach (var inserted in tobeInsertedArticles)
                {
                    // Query the 'new' articles sub set for duplicates i.e. different version of same arxivid
                    if (_articleContext.Articles.Any(a => a.ArxivId == inserted.ArxivId))
                    {
                        ///The scenario where an article to be 'inserted' has an existing version already inserted
                        ///in the dbConetxt witihn this processing session. The correct action is 'updated
                        var article = _articleContext.Articles.Where(a => a.ArxivId == inserted.ArxivId).FirstOrDefault();

                        if (article != null)
                        {
                            article.Update(inserted);
                            _articleContext.Articles.Attach(article);
                            updatedCount++;
                        }
                    }
                    else
                    {
                        _articleContext.Articles.Attach(inserted);
                        insertedCount++;
                    }
                }
            }
            stopwatch.Stop();

            // update journal fulfillment entry values (set as processed)
            newFulfillment.SetProcessingTime(stopwatch.ElapsedMilliseconds);
            newFulfillment.UpdateCounts(allArticles.Count, insertedCount, updatedCount,
                                        tobeInsertedArticles.Count + tobeUpdatedArticles.Count);
            newFulfillment.SetJobAsCompeleted();

            //return journal fulfillment id
            return _fulfillmentRepository.Save(newFulfillment).Id;
        }

        private List<Article> GetExistingArticlesByArxivId(List<string> arxivIds)
        {
            if (arxivIds == null | arxivIds.Count == 0)
                return new List<Article>();

            return _articleContext.Articles
                                .Include(x => x.AuthorArticles)
                                .Include(x => x.PaperVersions)
                                .Include(x => x.CategoryArticles)
                                .Where(x => arxivIds.Contains(x.ArxivId))
                                .OrderBy(x => x.ArxivId)
                                .ToList();
        }
    }
}
