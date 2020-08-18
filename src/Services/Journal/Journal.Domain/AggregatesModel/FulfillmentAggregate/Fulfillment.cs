using Journal.Domain.SeedWork;
using System;

namespace Journal.Domain.AggregatesModel.JobAggregate
{
    public class Fulfillment
        : Entity, IAggregateRoot
    {
        public string EventId { get; private set; }
        public string ExtractionFulfillmentId { get; private set; }
        public ProcessTypeEnum JournalType { get; private set; }
        public int ArticlesCount { get; private set; }

        public DateTime QueryFromDate { get; private set; }
        public DateTime QueryToDate { get; private set; }

        public int InsertedCount { get; private set; }
        public int UpdatedCount { get; private set; }
        public int TotalProcessedCount { get; private set; }
        public double ProcessingTimeInMilliseconds { get; private set; }

        public bool IsPending { get; private set; }
        public bool IsProcessed { get; private set; }
        public DateTime JobStartedDate { get; private set; }
        public DateTime JobCompletedDate { get; private set; }

        protected Fulfillment()
        {
            JobStartedDate = DateTime.MinValue;
            JobCompletedDate = DateTime.MinValue;
            JournalType = ProcessTypeEnum.Journal;
            IsPending = true;
        }

        public Fulfillment(string eventId, string extractionId, ProcessTypeEnum type,
            DateTime queryFromDate, DateTime queryToDate) : base()
        {
            EventId = eventId;
            ExtractionFulfillmentId = extractionId;
            JournalType = type;
            QueryFromDate = queryFromDate;
            QueryToDate = queryToDate;
        }

        public void UpdateCounts(int articles, int inserted, int updated, int total)
        {
            ArticlesCount = articles;
            InsertedCount = inserted;
            UpdatedCount = updated;
            TotalProcessedCount = total;
        }

        public void SetProcessingTime(long milliseconds)
        {
            ProcessingTimeInMilliseconds = milliseconds;
        }

        public void SetJobAsStarted()
        {
            JobStartedDate = DateTime.UtcNow;
        }

        public void SetJobAsCompeleted()
        {
            JobCompletedDate = DateTime.UtcNow;
            IsProcessed = true;
            IsPending = false;
        }

        public void SetJobAsFailed()
        {
            JobCompletedDate = DateTime.UtcNow;
            IsProcessed = false;
            IsPending = false;
        }
    }
}
