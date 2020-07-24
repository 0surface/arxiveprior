using System;

namespace arx.Extract.Types
{
    public interface IJobItem
    {
        string JobName { get; set; }//PK
        Guid JobItemId { get; set; }
        
        string QuerySubjectCode { get; set; }
        string QuerySubjectGroup { get; set; }
        int ItemsPerRequest { get; set; }
         int QueryDateInterval { get; set; }
    }

    public class JobItem : IJobItem
    {
        public string JobName { get; set; }//PK
        public Guid JobItemId { get; set; }
        public string QuerySubjectCode { get; set; }
        public string QuerySubjectGroup { get; set; }
        public int ItemsPerRequest { get; set; }
        public int QueryDateInterval { get; set; }
    }
}
