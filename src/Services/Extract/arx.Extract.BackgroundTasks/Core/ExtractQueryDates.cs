using System;

namespace arx.Extract.BackgroundTasks.Core
{
    public class ExtractQueryDates
    {
        public DateTime QueryFromDate { get; set; }
        public DateTime QueryToDate { get; set; }

        public ExtractQueryDates()
        {

        }
        public ExtractQueryDates(DateTime fromDate, DateTime toDate)
        {
            QueryFromDate = fromDate;
            QueryToDate = toDate;
        }
    }
}
