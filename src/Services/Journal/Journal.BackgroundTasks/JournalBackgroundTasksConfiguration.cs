namespace Journal.BackgroundTasks
{
    public class JournalBackgroundTasksConfiguration
    {
        public string ConnectionString { get; set; }
        public bool ArchiveModeIsActive { get; set; }
        public int PostProcessingWaitTimeInMilliSeconds { get; set; }
    }
}
