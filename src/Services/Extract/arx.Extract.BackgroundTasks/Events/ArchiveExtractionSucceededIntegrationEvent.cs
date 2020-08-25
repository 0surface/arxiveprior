using EventBus.Events;

namespace arx.Extract.BackgroundTasks.Events
{
    public class ArchiveExtractionSucceededIntegrationEvent : IntegrationEvent
    {
        public string ExtractionFulfillmentId { get; private set; }

        public ArchiveExtractionSucceededIntegrationEvent(string extractionFulfillmentId) : base()
            => ExtractionFulfillmentId = extractionFulfillmentId;
    }
}
