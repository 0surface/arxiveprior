using EventBus.Abstractions;
using Journal.API.Application.IntegrationEvents.Events;
using Journal.Domain.AggregatesModel.JobAggregate;
using Journal.Infrastructure;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.Threading.Tasks;

namespace Journal.API.Application.IntegrationEvents.EventHandling
{
    public class ArchiveExtractionSucceededIntegrationEventHandler :
        IIntegrationEventHandler<ArchiveExtractionSucceededIntegrationEvent>
    {

        private readonly ILogger<ArchiveExtractionSucceededIntegrationEventHandler> _logger;
        private readonly JournalContext _journalContext;

        public ArchiveExtractionSucceededIntegrationEventHandler(
            ILogger<ArchiveExtractionSucceededIntegrationEventHandler> logger,
            JournalContext journalContext)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            _journalContext = journalContext;
        }

        public async Task Handle(ArchiveExtractionSucceededIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

                Fulfillment newFulfillment = new Fulfillment(@event.Id.ToString(), @event.ExtractionFulfillmentId, ProcessTypeEnum.Archive);
                _journalContext.Fulfillments.Add(newFulfillment);
                _journalContext.SaveChanges();

                _logger.LogInformation("Created new Fulfillment from integration event: {IntegrationEventId} at {AppName}", @event.Id, Program.AppName);

                await Task.CompletedTask;
            }
        }
    }
}
