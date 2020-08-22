using EventBus.Events;
using Journal.Domain.AggregatesModel.JobAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journal.BackgroundTasks.Events
{
    public class JournalProcessFailedIntegrationEvent : IntegrationEvent
    {
        public string ExtractionFulfillmentId { get; set; }
        public ProcessTypeEnum JournalType { get; set; }
        public JournalProcessFailedIntegrationEvent(string extractionFulfillmentId) => ExtractionFulfillmentId = extractionFulfillmentId;
    }
}
