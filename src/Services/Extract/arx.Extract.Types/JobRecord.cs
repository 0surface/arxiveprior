using System;

namespace arx.Extract.Types
{
    public class JobRecord
    {
        public Guid JobId { get; set; }
        public Guid JobRecordId { get; set; }
        public ExtractTypeEnum Type { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime CompletedDate { get; set; }
        public DateTime QueryFromDate { get; set; }
        public DateTime QueryToDate { get; set; }
        public string QuerySubjectCode { get; set; }
        public string QuerySubjectGroup { get; set; }
        public int TotalCount { get; set; }
        public bool PartialSuccess { get; set; }
        public bool CompleteSuccess { get; set; }
    }
}
