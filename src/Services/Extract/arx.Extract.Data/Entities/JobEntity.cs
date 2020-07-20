using arx.Extract.Types;
using Microsoft.Azure.Cosmos.Table;
using System;

namespace arx.Extract.Data.Entities
{
    public class JobEntity : TableEntity
    {
        public ExtractTypeEnum Type { get; set; }
        public string Name { get; set; }
        public Guid JobId { get; set; }
        public string Description { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string QueryBaseUrl { get; set; }
    }
}
