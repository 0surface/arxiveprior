namespace arx.Extract.BackgroundTasks.Events
{
    public class ExtractionCompletedIntegrationEvent
    {
        public string ExtractionId { get; set; }

        public ExtractionCompletedIntegrationEvent(string extractionId) => ExtractionId = extractionId;

    }
}
