using Microsoft.Azure.Cosmos.Table;
using System;

namespace arx.Extract.Data.Entities
{
    public class TaskRecordEntity : TableEntity
    {
        public string TaskItemId { get; set; }
        public Guid TaskRecordId { get; set; }
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
