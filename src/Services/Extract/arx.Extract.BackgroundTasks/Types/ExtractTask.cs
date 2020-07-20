using System;
using System.Collections.Generic;
using System.Text;

namespace arx.Extract.BackgroundTasks.Types
{
    public class ExtractTask
    {
        public ExtractTypeEnum Type { get; set; }
        public string  Name { get; set; }
        public string  Description { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string QueryBaseUrl { get; set; }
    }
}
