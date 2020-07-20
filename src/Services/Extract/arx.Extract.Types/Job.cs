using System;

namespace arx.Extract.Types
{
    public class Job
    {
        public ExtractTypeEnum Type { get; set; }
        public string Name { get; set; }
        public Guid JobId { get; set; }        
        public string Description { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string QueryBaseUrl { get; set; }
    }
}
