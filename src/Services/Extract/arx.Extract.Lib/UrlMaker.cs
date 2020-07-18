namespace arx.Extract.Lib
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
            string dateQueryString = $"+AND+submittedDate:[{start}+TO+{end}]";

            return $"{baseUrl}{subjectQuery}{dateQueryString}{maxResultsQuery}";
        }
    }
}
