using arx.Extract.Types;
using Microsoft.Azure.Cosmos.Table;
using System;

namespace arx.Extract.Data.Entities
{
    public class FulfilmentEntity : TableEntity, IFulfilment
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
