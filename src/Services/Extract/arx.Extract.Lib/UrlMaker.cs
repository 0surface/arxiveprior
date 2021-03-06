﻿namespace arx.Extract.Lib
{
    public class UrlMaker
    {
        public static string RequestUrlForItemsBetweenDates(UrlParams urlParams)
        {
            string maxResultsQuery = urlParams.ItemsPerRequest > 0 ?
                                       $"&max_results={urlParams.ItemsPerRequest}"
                                      : "0";
            string baseUrl = string.IsNullOrEmpty(urlParams.QueryBaseUrl) ?
                                string.Empty
                                : "http://export.arxiv.org/api/query?search_query=";

            string subjectQuery = urlParams.SubjectCode.Contains(".") ?
                                $"cat:{ urlParams.SubjectCode}"
                                : $"cat:{ urlParams.SubjectCode}*";

            string start = urlParams?.QueryFromDate.ToString("yyyyMMddHHmm");
            string end = urlParams?.QueryToDate.ToString("yyyyMMddHHmm");
            string dateQueryString = $"+AND+lastUpdatedDate:[{start}+TO+{end}]";
            string startIndex = urlParams?.StartIndex == 0 ? "" : $"&start={urlParams?.StartIndex}";

            return $"{baseUrl}{subjectQuery}{dateQueryString}{startIndex}{maxResultsQuery}";
        }

        public static string FulfillmentUrlBetweenDates(UrlParams urlParams)
        {
            string maxResultsQuery = urlParams.ItemsPerRequest > 0 ?
                                       $"&max_results={urlParams.ItemsPerRequest}"
                                      : "0";
            string baseUrl = string.IsNullOrEmpty(urlParams.QueryBaseUrl) ?
                                string.Empty
                                : "http://export.arxiv.org/api/query?search_query=";

            string subjectQuery = string.IsNullOrEmpty(urlParams.SubjectCode) ?
                                $"cat:{urlParams.SubjectGroupCode}*" :
                                $"cat:{urlParams.SubjectCode}";

            string start = urlParams?.QueryFromDate.ToString("yyyyMMddHHmm");
            string end = urlParams?.QueryToDate.ToString("yyyyMMddHHmm");
            string dateQueryString = $"+AND+lastUpdatedDate:[{start}+TO+{end}]";
            string startIndex = urlParams?.StartIndex == 0 ? "" : $"&start={urlParams?.StartIndex}";

            return $"{baseUrl}{subjectQuery}{dateQueryString}{startIndex}{maxResultsQuery}";
        }
    }
}
