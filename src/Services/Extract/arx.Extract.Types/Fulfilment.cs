using System;

namespace arx.Extract.Types
{
    public interface IFulfillment
    {
         string JobName { get; set; } //PK
         Guid FulfillmentId { get; set; } //RK
         ExtractTypeEnum Type { get; set; }
         DateTime QueryFromDate { get; set; }
         DateTime QueryToDate { get; set; }

         DateTime JobStartedDate { get; set; }
         DateTime JobCompletedDate { get; set; }
         double ProcessingTimeInSeconds { get; set; }
         int TotalCount { get; set; }
         bool PartialSuccess { get; set; }
         bool CompleteSuccess { get; set; }
    }
    public class Fulfillment : IFulfillment
    {
        public string JobName { get; set; } //PK
        public Guid FulfillmentId { get; set; } //RK
        public ExtractTypeEnum Type { get; set; }
        public DateTime QueryFromDate { get; set; }
        public DateTime QueryToDate { get; set; }

        public DateTime JobStartedDate { get; set; }
        public DateTime JobCompletedDate { get; set; }
        public double ProcessingTimeInSeconds { get; set; }
        public int TotalCount { get; set; }
        public bool PartialSuccess { get; set; }
        public bool CompleteSuccess { get; set; }
    }
}
