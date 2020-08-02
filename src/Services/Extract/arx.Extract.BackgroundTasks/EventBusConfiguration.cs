﻿namespace arx.Extract.BackgroundTasks
{
    public class EventBusConfiguration
    {
        public bool AzureServiceBusEnabled { get; set; }
        public string SubscriptionClientName { get; set; }

        public int EventBusRetryCount { get; set; }
        public string EventBusConnection { get; set; }
        public string EventBusUserName { get; set; }
        public string EventBusPassword { get; set; }
    }


}
