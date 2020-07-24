using arx.Extract.Types;
using Microsoft.Azure.Cosmos.Table;
using System;

namespace arx.Extract.Data.Entities
{
    public class FulfilmentItemEntity : TableEntity, IFulfilmentItem
    {
        public Guid FulfilmentId { get; set; } //PK
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
