﻿using System;
using System.Collections.Generic;
using System.Text;

namespace arx.Extract.BackgroundTasks
{
    public class BackgroundTaskSettings
    {
        public string  ConnectionString { get; set; }
        public string EventBusConnection { get; set; }
        public string SubscriptionClientName { get; set; }
        public int PostFetchWaitTime { get; set; }
        public string StorageConnectionString { get; set; }

        public string SubjectTableName { get; set; }
    }
}