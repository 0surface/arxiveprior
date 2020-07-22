using arx.Extract.Types;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;

namespace arx.Extract.Data.Entities
{
    public class JobEntity : TableEntity, IJob
    {
        public ExtractTypeEnum Type { get; set; }//PK
        public string UniqueName { get; set; } //RK
        public string Description { get; set; }
        public string QueryBaseUrl { get; set; }
    }
}
