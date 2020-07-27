namespace arx.Extract.BackgroundTasks
{
    public class BackgroundTaskSettings
    {
        public string ConnectionString { get; set; }
        public string EventBusConnection { get; set; }
        public string SubscriptionClientName { get; set; }
        public int PostFetchWaitTime { get; set; }
        public string StorageConnectionString { get; set; }

        public string SubjectTableName { get; set; }
        public string PublicationTableName { get; set; }
        public string JobTableName { get; set; }
        public string JobItemTableName { get; set; }
        public string FulfillmentTableName { get; set; }
        public string FulfillmentItemTableName { get; set; }

        public string ExtractionMode { get; set; }
        public bool ArchiveModeIsActive { get; set; }
        public string ArchiveJobName { get; set; }
        public string SeedJobName { get; set; }
        public string JournalJobName { get; set; }

        public int ArxivApiPagingRequestDelay { get; set; }
    }
}
