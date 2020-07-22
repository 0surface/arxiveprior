using arx.Extract.Data.Common;
using arx.Extract.Types;
using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;

namespace arx.Extract.Data.Entities
{
    public class JobEntity : TableEntity, IJob
    {
        [EntityEnumPropertyConverter]
        public ExtractTypeEnum Type { get; set; }//PK
        public string UniqueName { get; set; } //RK
        public string Description { get; set; }
        public string QueryBaseUrl { get; set; }
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
