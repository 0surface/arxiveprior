using System;

namespace arx.Extract.Types
{
    public class JobRecordItem
    {
        public string JobRecordId { get; set; }
        public Guid RecordItemId { get; set; }
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
