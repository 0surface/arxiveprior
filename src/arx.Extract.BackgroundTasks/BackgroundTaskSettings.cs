using System;
using System.Collections.Generic;
using System.Text;

namespace arx.Extract.BackgroundTasks
{
    public class BackgroundTaskSettings
    {
        public string  ConnectionString { get; set; }
        public string EventBusConnection { get; set; }
        public int PostFetchWaitTime { get; set; }
    }
}
