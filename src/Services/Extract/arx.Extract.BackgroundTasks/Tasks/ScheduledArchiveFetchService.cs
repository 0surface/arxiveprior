using arx.Extract.BackgroundTasks.Events;
using EventBus.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace arx.Extract.BackgroundTasks.Tasks
{

    public class ScheduledArchiveService : BackgroundService
    {
        private readonly BackgroundTaskSettings _settings;
        private readonly IEventBus _eventBus;
        private readonly ILogger<ScheduledArchiveService> _logger;

        public ScheduledArchiveService(IOptions<BackgroundTaskSettings> settings,
            IEventBus eventBus,
            ILogger<ScheduledArchiveService> logger)
        {
            _settings = settings?.Value ?? throw new ArgumentException(nameof(settings));
            _eventBus = eventBus;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); ;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("ScheduledArchiveFetchService background task is starting.");

            stoppingToken.Register(() => _logger.LogDebug("#1 ScheduledArchiveFetchService background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug($"ScheduledArchiveFetchService background task is doing background work. [{DateTime.UtcNow}]");

                RunScheduledArchiveService();
                _logger.LogInformation($"_settings.PostFetchWaitTime : {_settings.PostFetchWaitTime}");
                await Task.Delay(/*_settings.PostFetchWaitTime*/10000, stoppingToken);
            }

            _logger.LogDebug("ScheduledArchiveFetchService background task is stopping.");

            await Task.CompletedTask;
        }

        private void RunScheduledArchiveService()
        {
            _logger.LogDebug("Running Scheduled Archive Service.");

            //TODO connec to db read last archive task, determine next task's jobs' params
            _logger.LogDebug("Reading Metadata to determine archive task parameters.");

            var extractId = Guid.NewGuid().ToString();

            _logger.LogDebug("Fetching data from Http requests and persisting to Store.");
            //TODO  Loop through each subjectcode/url request cycle


            var extractionCompletedEvent = new ExtractionCompletedIntegrationEvent(extractId);

            _logger.LogInformation("----- Publishing Integration Event: {IntegrationEventId} from {AppName} = ({@IntegrationEvent})",
                                    extractionCompletedEvent.ExtractionId, Program.AppName, extractionCompletedEvent);

            //TODO: Publish to eventBus
            _eventBus.Publish(extractionCompletedEvent);
        }
    }
}
