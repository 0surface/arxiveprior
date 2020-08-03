using Journal.Domain.SeedWork;
using System;

namespace Journal.Domain.AggregatesModel.JobAggregate
{
    public class Fulfillment
        : Entity, IAggregateRoot
    {
        public string EventId { get; set; }
        public string ExtractionFulfillmentId { get; set; }
        public ProcessTypeEnum JournalType { get; set; }
        public int ArticlesCount { get; set; }

        public int InsertedCount { get; set; }
        public int UpdatedCount { get; set; }
        public int TotalProcessedCount { get; set; }
        public int ProcessingTimeInMilliseconds { get; set; }

        public DateTime JobStartedDate { get; set; }
        public DateTime JobCompletedDate { get; set; }
    }
}
