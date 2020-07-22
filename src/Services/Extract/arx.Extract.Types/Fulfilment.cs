using System;

namespace arx.Extract.Types
{
    public interface IFulfilment
    {
         string JobName { get; set; } //PK
         Guid FulfilmentId { get; set; } //RK
         ExtractTypeEnum Type { get; set; }
         DateTime QueryFromDate { get; set; }
         DateTime QueryToDate { get; set; }

         DateTime JobStartedDate { get; set; }
         DateTime JobCompletedDate { get; set; }
         int TotalCount { get; set; }
         bool PartialSuccess { get; set; }
         bool CompleteSuccess { get; set; }
    }
    public class Fulfilment : IFulfilment
    {
        public string JobName { get; set; } //PK
        public Guid FulfilmentId { get; set; } //RK
        public ExtractTypeEnum Type { get; set; }
        public DateTime QueryFromDate { get; set; }
        public DateTime QueryToDate { get; set; }

        public DateTime JobStartedDate { get; set; }
        public DateTime JobCompletedDate { get; set; }
        public int TotalCount { get; set; }
        public bool PartialSuccess { get; set; }
        public bool CompleteSuccess { get; set; }
    }
}
