using arx.Extract.Types;
using Microsoft.Azure.Cosmos.Table;
using System;

namespace arx.Extract.Data.Entities
{
    public class FulfilmentEntity : TableEntity
    {
        public Guid JobId { get; set; }
        public Guid FulfilmentId { get; set; }
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
