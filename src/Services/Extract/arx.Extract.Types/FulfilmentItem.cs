using System;

namespace arx.Extract.Types
{
    public interface IFulfillmentItem 
    {
         Guid FulfillmentId { get; set; } //PK
         Guid ItemUId { get; set; }//RK
         Guid JobItemId { get; set; }
         string QuerySubjectCode { get; set; }
         string QuerySubjectGroup { get; set; }
         int ItemsPerRequest { get; set; }
         string Url { get; set; }

         DateTime JobItemStartDate { get; set; }
         DateTime JobItemCompletedDate { get; set; }
         double TotalProcessingInMilliseconds { get; set; }
         bool HttpRequestIsSuccess { get; set; }
         bool DataExtractionIsSuccess { get; set; }
         int HttpRequestCount { get; set; }
         int TotalResults { get; set; }
         int ResultSizePerHttpRequest { get; set; }
         double FetchTimeSpan { get; set; }
         int DelayBetweenHttpRequests { get; set; }
    }


    public class FulfillmentItem : IFulfillmentItem
    {
        public Guid FulfillmentId { get; set; } //PK
        public Guid ItemUId { get; set; }//RK
        public Guid JobItemId { get; set; }
        public string QuerySubjectCode { get; set; }
        public string QuerySubjectGroup { get; set; }
        public int ItemsPerRequest { get; set; }
        public string Url { get; set; }

        public DateTime JobItemStartDate { get; set; }
        public DateTime JobItemCompletedDate { get; set; }
        public double TotalProcessingInMilliseconds { get; set; }
        public bool HttpRequestIsSuccess { get; set; }
        public bool DataExtractionIsSuccess { get; set; }
        public int HttpRequestCount { get; set; }
        public int TotalResults { get; set; }
        public int ResultSizePerHttpRequest { get; set; }
        public double FetchTimeSpan { get; set; }
        public int DelayBetweenHttpRequests { get; set; }
    }
}
