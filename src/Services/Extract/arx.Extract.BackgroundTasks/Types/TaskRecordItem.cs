using System;

namespace arx.Extract.BackgroundTasks.Types
{
    public class TaskRecordItem
    {
        public string TaskRecordId { get; set; }
        public Guid ItemUId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool HttpRequestIsSuccess { get; set; }
        public bool DataExtractionIsSuccess { get; set; }
        public string Url { get; set; }
        public int HttpRequestCount { get; set; }
        public int TotalResults { get; set; }
        public int ResultSizePerHttpRequest { get; set; }
        public int FetchTimeSpan { get; set; }
        public int DelayBetweenHttpRequests { get; set; }
    }
}
