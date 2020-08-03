using EventBus.Events;
using Journal.Domain.AggregatesModel.JobAggregate;

namespace Journal.BackgroundTasks.Events
{
    public class JournalProcessedIntegrationEvent : IntegrationEvent
    {
        public int JournalId { get; set; }
        public JournalTypeEnum JournalType { get; set; }
        public JournalProcessedIntegrationEvent(int journalId) => JournalId = journalId;
    }
}
