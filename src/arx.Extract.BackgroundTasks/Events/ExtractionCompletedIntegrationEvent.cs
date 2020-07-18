using EventBus.Events;

namespace arx.Extract.BackgroundTasks.Events
{
    public class ExtractionCompletedIntegrationEvent : IntegrationEvent
    {
        public string ExtractionId { get; set; }

        public ExtractionCompletedIntegrationEvent(string extractionId) => ExtractionId = extractionId;
    }
}
