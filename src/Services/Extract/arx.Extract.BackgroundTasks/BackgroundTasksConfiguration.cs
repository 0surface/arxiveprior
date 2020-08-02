using System;

namespace arx.Extract.BackgroundTasks
{
    public class BackgroundTasksConfiguration
    {
        public string ExtractionMode { get; set; }
        public bool ArchiveModeIsActive { get; set; }
        public DateTime ArchiveTerminateDate { get; set; }
        public string ArchiveJobName { get; set; }
        public string SeedJobName { get; set; }
        public string JournalJobName { get; set; }

        public int PostFetchWaitTime { get; set; }
        public int ArxivApiPagingRequestDelay { get; set; }
    }


}
