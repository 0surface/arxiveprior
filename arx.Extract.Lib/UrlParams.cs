using System;

namespace arx.Extract.Lib
{
    public class UrlParams
    {
        public string SubjectCode { get; set; }
        public DateTime QueryFromDate { get; set; }
        public DateTime QueryToDate { get; set; }
        public int ItemsPerRequest { get; set; }
        public int StartIndex { get; set; }
        public string QueryBaseUrl { get; set; }
    }
}
