using arx.Extract.Types;
using Microsoft.Azure.Cosmos.Table;
using System;

namespace arx.Extract.Data.Entities
{
    public class JobItemEntity : TableEntity, IJobItem
    {
        public string JobName { get; set; }//PK
        public Guid JobItemId { get; set; } //RK
        public string QuerySubjectCode { get; set; }
        public string QuerySubjectGroup { get; set; }
        public int ItemsPerRequest { get; set; }
        public int QueryDateInterval { get; set; }
        public DateTime ArchiveTerminationDate { get; set; }
    }
}
