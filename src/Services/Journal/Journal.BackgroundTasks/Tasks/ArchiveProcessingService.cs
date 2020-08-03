using EventBus.Abstractions;
using Journal.BackgroundTasks.Events;
using Journal.Domain.AggregatesModel.JobAggregate;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Journal.BackgroundTasks.Tasks
{
    public class ArchiveJournalProcessingService : BackgroundService
    {
        private readonly ILogger<ArchiveJournalProcessingService> _logger;
        private readonly IEventBus _eventBus;
        private readonly JournalBackgroundTasksConfiguration _config;

        public string AppName { get; set; } = typeof(ArchiveJournalProcessingService).Name;

        public ArchiveJournalProcessingService(ILogger<ArchiveJournalProcessingService> logger,
            IOptions<JournalBackgroundTasksConfiguration> config,
            IEventBus eventBus)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventBus = eventBus;
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

        private Task<int> RunArchiveJournalProcessing(CancellationToken stoppingToken)
        {
            return Task.FromResult(42);
        }
    }
}
