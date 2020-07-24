using arx.Extract.Data.Common;
using arx.Extract.Types;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;

namespace arx.Extract.Data.Entities
{
    public class FulfilmentEntity : TableEntity, IFulfilment
    { 
        public string JobName { get; set; } //PK
        public Guid FulfilmentId { get; set; } //RK
        [EntityEnumPropertyConverter]
        public ExtractTypeEnum Type { get; set; }        
        public DateTime QueryFromDate { get; set; }
        public DateTime QueryToDate { get; set; }

        public DateTime JobStartedDate { get; set; }
        public DateTime JobCompletedDate { get; set; }
        public int TotalCount { get; set; }
        public bool PartialSuccess { get; set; }
        public bool CompleteSuccess { get; set; }

        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var results = base.WriteEntity(operationContext);
            EntityEnumPropertyConverter.Serialize(this, results);
            return results;
        }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);
            EntityEnumPropertyConverter.Deserialize(this, properties);
        }
    }
}
