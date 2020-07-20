using System;

namespace arx.Extract.Types
{
    public class JobItem
    {
        public Guid JobId { get; set; }
        public Guid JobItemId { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string QuerySubjectCode { get; set; }
        public string QuerySubjectGroup { get; set; }
        public int ItemsPerRequest { get; set; }
        public string QueryBaseUrl { get; set; }
    }
}
