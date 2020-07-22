using System;

namespace arx.Extract.Types
{
    public interface IJob
    {
         ExtractTypeEnum Type { get; set; }
         string UniqueName { get; set; }
         string Description { get; set; }
         string QueryBaseUrl { get; set; }
    }
    public class Job : IJob
    {
        public ExtractTypeEnum Type { get; set; }
        public string UniqueName { get; set; }
        public string Description { get; set; }
        public string QueryBaseUrl { get; set; }
    }
}
