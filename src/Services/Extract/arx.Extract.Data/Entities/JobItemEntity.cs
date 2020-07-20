using Microsoft.Azure.Cosmos.Table;
using System;

namespace arx.Extract.Data.Entities
{
    public class JobItemEntity : TableEntity
    {   public Guid JobItemId { get; set; }
        public Guid ItemUId { get; set; }
        public DateTime LastModifiedDate { get; set; }

        public string Url { get; set; }
        public int QueryDateInterval { get; set; }
        public string QuerySubjectCode { get; set; }
        public string QuerySubjectGroup { get; set; }
        public int ItemsPerRequest { get; set; }
        public string QueryBaseUrl { get; set; }
    }
}
