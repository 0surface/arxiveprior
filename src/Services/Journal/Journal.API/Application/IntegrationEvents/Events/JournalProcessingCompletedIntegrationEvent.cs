using EventBus.Events;
using System;

namespace Journal.API.Application.IntegrationEvents.Events
{
    public class JournalProcessingCompletedIntegrationEvent : IntegrationEvent
    {
        public string JournalId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }


        public JournalProcessingCompletedIntegrationEvent(string journalId) => JournalId = journalId;
    }
}
