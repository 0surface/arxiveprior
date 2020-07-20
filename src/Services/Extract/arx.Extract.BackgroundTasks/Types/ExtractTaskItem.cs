using System;

namespace arx.Extract.BackgroundTasks.Types
{
    public class ExtractTaskItem
    {
        public string TaskId { get; set; }
        public Guid ItemUId { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string QuerySubjectCode { get; set; }
        public string QuerySubjectGroup { get; set; }
        public int ItemsPerRequest { get; set; }
        public string QueryBaseUrl { get; set; }
    }
}
