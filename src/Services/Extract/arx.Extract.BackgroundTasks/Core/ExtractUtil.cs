using arx.Extract.Data.Entities;
using arx.Extract.Lib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace arx.Extract.BackgroundTasks.Core
{
    public static class ExtractUtil
    {
        /// <summary>
        /// Returns the number of Http requests required to fetch all items based on the 
        /// total available and currently fetched values.
        /// </summary>
        /// <param name="totalAvailable">int</param>
        /// <param name="fetched">int</param>
        /// <returns>Number of pending Requests</returns>
        public static int CalculatePagedRequestCount(int totalAvailable, int fetched)
        {
            if (fetched == 0)
                return 1;

            int yetToFetch = (totalAvailable - fetched);

            if ((yetToFetch / fetched) < 1)
                return 1;

            int requests = (int)Math.Floor((double)(yetToFetch / fetched));

            if (yetToFetch % fetched > 0) requests++;

            return requests;
        }

        /// <summary>
        /// Returns date interval as int, from, to as DateTime values.
        /// Calculates next set of values for Archive operations.
        /// If lastFulfillment is not found, defaults to current UTC day minus 2
        /// as the start of the Archive Services' Query date.
        /// </summary>
        /// <param name="lastFulfillment"></param>
        /// <param name="queryDateInterval"></param>
        /// <returns>(int, DateTime, DateTime)</returns>
        public static (int, DateTime, DateTime) CalculateArchiveQueryDates
                        (DateTime lastFulfillmentQueryFromDate, DateTime lastFulfillmentQueryToDate, int queryDateInterval)
        {
            if (lastFulfillmentQueryFromDate == DateTime.MinValue || lastFulfillmentQueryToDate == DateTime.MinValue)
            {
                //Start from Two days before current DateTime.
                DateTime initDate = DateTime.UtcNow.Date.AddDays(-2);                
                var (initFrom, initTo) = GetArchiveDates(initDate, queryDateInterval);
                return (queryDateInterval, initFrom, initTo);
            }

            TimeSpan? span = lastFulfillmentQueryToDate - lastFulfillmentQueryFromDate;

            //Default to 1 day intervals in case the span is not found (e.g. appsettings misread incorrectly)
            int lastFulfillmentSpanDays = (span.HasValue && span.Value.Days != 0) ? Math.Abs(span.Value.Days) : 1;

            var (queryFrom, queryTo) = GetArchiveDates(lastFulfillmentQueryFromDate, lastFulfillmentSpanDays);

            return (lastFulfillmentSpanDays, queryFrom, queryTo);
        }

        /// <summary>
        ///  Returns Archive Query From and To Dates from the given span and reference (FromDate) values.
        ///  Expects a non null and valid referenceDate value as parameter.
        ///  Expects a non zero  querySpanInDays value
        /// </summary>
        /// <param name="referenceDate"></param>
        /// <param name="querySpanInDays"></param>
        /// <returns>Tuple<DateTime,DateTime></returns>
        private static (DateTime fromDate, DateTime toDate) GetArchiveDates (DateTime referenceDate, int querySpanInDays) 
        {
            /* 1. Use the last 'From Date'  as a reference Date. 
             * 2. Make the current Archive 'toDate' start just one second before the reference Date
             * 3. Make the current Archive 'fromDate' start n days before the reference Date(where n = querySpanInDays)
             * (2) & (3) ensure that the 'toDate' values have HH:mm:ss = 23:59:59 & 'fromDate' values have HH:mm:ss = 00:00:00
             */
            DateTime queryToDate = referenceDate.AddSeconds(-1);
            DateTime queryFromDate = referenceDate.AddDays(-1 * querySpanInDays).Date;
            return (queryFromDate, queryToDate);
        }

        /// <summary>
        /// Returns a list of From, To datetime tuple values.
        /// Returns a list of calcualted  From, To date tuple values, if the given query Date interval compared to the last fulfillment interval is not optimal.
        /// Otherwise resturns one set of (From,To) dates callcuated from the given queryDate Interval.
        /// </summary>
        /// <param name="lastFulfillment">FulfillmentEntity</param>
        /// <param name="queryDateInterval">int</param>
        /// <returns>List<Tuple<DateTime, DateTime>></returns>
        public static List<ExtractQueryDates> GetRequestChunkedArchiveDates(FulfillmentEntity lastFulfillment, int queryDateInterval)
        {
            /* Tuple(FromDate,ToDate) */
            List<ExtractQueryDates> result = new List<ExtractQueryDates>();

            try
            {
                var (lastSpanDays, nextFromDate, nextToDate) =
                    CalculateArchiveQueryDates(lastFulfillment.QueryFromDate, lastFulfillment.QueryToDate, queryDateInterval);

                /* queryDateInterval is optimal */
                if (queryDateInterval >= lastSpanDays)
                {
                    result.Add(new ExtractQueryDates(nextFromDate, nextToDate));
                    return result;
                }

                /* lastSpanDays is too big, needs to be chuncked. 
                 * Requires more than one set of (from, to) query dates / fulfillment items */

                DateTime referenceDate = nextToDate.AddSeconds(1);

                int chunkSize = queryDateInterval;

                int remainder = lastSpanDays % queryDateInterval;

                int noOfChunks = remainder == 0 ?
                    lastSpanDays / queryDateInterval
                    : ((lastSpanDays - remainder) / queryDateInterval);


                for (int i = 0; i < noOfChunks; i++)
                {
                    if (i == 0)
                    {
                        var (initFromDate, initToDate) = GetArchiveDates(referenceDate, chunkSize);
                        result.Add(new ExtractQueryDates(initFromDate, initToDate));
                    }
                    else
                    {
                        var (fromDate, toDate) = GetArchiveDates(result[i - 1].QueryFromDate, chunkSize);
                        result.Add(new ExtractQueryDates(fromDate, toDate));
                    }
                }
                if (remainder != 0)
                {
                    /* Since chunk count is an odd number, add the last Query date interval Chunck */
                    int remainderIntervalInDays = (result.Last().QueryFromDate - nextFromDate).Days;
                    var (lastfromDate, lastToDate) = GetArchiveDates(result.Last().QueryFromDate, remainderIntervalInDays);
                    result.Add(new ExtractQueryDates(lastfromDate, lastToDate));
                }
            }
            catch (Exception)
            {
            }

            return result;
        }

        public static FulfillmentItemEntity MakeNewFulfillmentItem(JobItemEntity jobItem, ExtractQueryDates requestDateInterval, string baseUrl, Guid newFulfillmentId)
        {
            FulfillmentItemEntity fulfillmentItem = new FulfillmentItemEntity()
            {
                FulfillmentId = newFulfillmentId, //PK
                ItemUId = Guid.NewGuid(), //RK
                JobItemId = jobItem.JobItemId,
                QuerySubjectCode = jobItem.QuerySubjectCode,
                QuerySubjectGroup = jobItem.QuerySubjectGroup,
                ItemsPerRequest = jobItem.ItemsPerRequest,
                JobItemStartDate = new DateTime(1970, 01, 01),
                JobItemCompletedDate = new DateTime(1970, 01, 01),
            };

            fulfillmentItem.PartitionKey = fulfillmentItem.FulfillmentId.ToString();
            fulfillmentItem.RowKey = fulfillmentItem.ItemUId.ToString();
            fulfillmentItem.Url = ConstructRequestUrl(requestDateInterval, baseUrl, fulfillmentItem);

            return fulfillmentItem;
        }

        public static FulfillmentEntity MakeNewFulfillment(JobEntity job, FulfillmentEntity lastFulfillment, int averageQueryDateInterval)
        {
            FulfillmentEntity item = new FulfillmentEntity()
            {
                JobName = job.UniqueName,
                FulfillmentId = Guid.NewGuid(),
                Type = job.Type,
                JobStartedDate = DateTime.UtcNow,
                JobCompletedDate = new DateTime(1970, 01, 01)
            };

            item.PartitionKey = item.JobName;
            item.RowKey = item.FulfillmentId.ToString();

            var (_, fromDate, toDate) = ExtractUtil.CalculateArchiveQueryDates
                (lastFulfillment.QueryFromDate, lastFulfillment.QueryToDate, averageQueryDateInterval);

            item.QueryFromDate = fromDate;
            item.QueryToDate = toDate;

            return item;
        }

        public static string ConstructRequestUrl(ExtractQueryDates queryDates, string baseUrl, FulfillmentItemEntity fulfillmentItem)
        {
            UrlParams urlParams = new UrlParams()
            {
                SubjectCode = fulfillmentItem.QuerySubjectCode,
                SubjectGroupCode = fulfillmentItem.QuerySubjectGroup,
                QueryFromDate = queryDates.QueryFromDate,
                QueryToDate = queryDates.QueryToDate,
                ItemsPerRequest = fulfillmentItem.ItemsPerRequest,
                QueryBaseUrl = baseUrl
            };

            return UrlMaker.FulfillmentUrlBetweenDates(urlParams);
        }

        public static bool HasPassedTerminationDate(DateTime terminationDate, DateTime toDate)
        {
            if (terminationDate == null || terminationDate == DateTime.MinValue ||
                toDate == null || toDate == DateTime.MinValue)
                return true;

            return (toDate - terminationDate).Days < 0;
        }
    }
}
