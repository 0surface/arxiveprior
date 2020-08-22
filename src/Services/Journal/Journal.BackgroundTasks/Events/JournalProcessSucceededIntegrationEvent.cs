using EventBus.Events;
using Journal.Domain.AggregatesModel.JobAggregate;

namespace Journal.BackgroundTasks.Events
{
    public class JournalProcessSucceededIntegrationEvent : IntegrationEvent
    {
        public int JournalId { get; set; }
        public ProcessTypeEnum JournalType { get; set; }
        public JournalProcessSucceededIntegrationEvent(int journalId) => JournalId = journalId;
    }
}
