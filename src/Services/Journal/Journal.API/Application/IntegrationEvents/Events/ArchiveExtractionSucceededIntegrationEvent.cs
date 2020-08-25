using EventBus.Events;

namespace Journal.API.Application.IntegrationEvents.Events
{
    public class ArchiveExtractionSucceededIntegrationEvent : IntegrationEvent
    {
        public string ExtractionFulfillmentId { get; private set; }

        public ArchiveExtractionSucceededIntegrationEvent(string extractionFulfillmentId) : base()
            => ExtractionFulfillmentId = extractionFulfillmentId;
    }
}
