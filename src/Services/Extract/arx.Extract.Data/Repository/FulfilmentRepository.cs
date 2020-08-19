using arx.Extract.Data.Common;
using arx.Extract.Data.Entities;
using arx.Extract.Types;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace arx.Extract.Data.Repository
{
    public interface IFulfillmentRepository
    {
        Task<FulfillmentEntity> SaveFulfillment(FulfillmentEntity jobRecord);

        Task<List<FulfillmentEntity>> GetFulfillments(string jobName);
        Task<List<FulfillmentEntity>> GetFulfillmentsBetweenQueryDates(string jobName, DateTime queryFromDate, DateTime queryToDate);
        Task<FulfillmentEntity> GetLastSuccessfulFulfillment(string jobName);
        Task<FulfillmentEntity> GetLastSuccessfulArchiveFulfillment(DateTime queryFromDate);
        Task<List<FulfillmentEntity>> GetFailedFulfillments(string jobName);
    }
    public class FulfillmentRepository : TableStorage, IFulfillmentRepository
    {
        public FulfillmentRepository(string connectionString, string tableName) : base(tableName, connectionString)
        {
            Reference.CreateIfNotExists();
        }

        public async Task<FulfillmentEntity> SaveFulfillment(FulfillmentEntity fulfillment)
        {
            try
            {
                var response = await InsertOrReplace(fulfillment);
                return (FulfillmentEntity)response.Result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<FulfillmentEntity>> GetFulfillments(string jobName)
        {
            var response = await QueryByPartition<FulfillmentEntity>(jobName);

            return response?.ToList() ?? new List<FulfillmentEntity>();
        }

        public async Task<List<FulfillmentEntity>> GetFulfillmentsBetweenQueryDates(string jobName, DateTime queryFromDate, DateTime queryToDate)
        {
            var t = typeof(FulfillmentEntity);
            var conditions = new List<QueryFilterCondition>()
            {
                new QueryFilterCondition (string.Empty, t.GetProperty("JobName").Name, QueryComparisons.Equal, jobName),
                new QueryFilterCondition ("datetime", t.GetProperty("QueryFromDate").Name, QueryComparisons.GreaterThanOrEqual, queryFromDate),
                new QueryFilterCondition ("datetime", t.GetProperty("QueryToDate").Name, QueryComparisons.LessThanOrEqual, queryToDate),
            };

            var tableQuery = QueryFilterUtil.AndQueryFilters<FulfillmentEntity>(conditions);

            var response = await Query(tableQuery);

            return response
                    ?.OrderByDescending(x => x.QueryFromDate)
                    ?.ToList()
                    ?? new List<FulfillmentEntity>();
        }

        public async Task<FulfillmentEntity> GetLastSuccessfulArchiveFulfillment(DateTime queryFromDate)
        {
            var t = typeof(FulfillmentEntity);

            ///Walk 5 seconds away from the given queryFromDate in both directions, 
            /// to find the desired queryToDate which should only be 1 second behind the queryFromDate value.
            /// Example, if queryFromDate = 02/08/2020 00:00:00 then the query is in this range [01/08/2020 23:59:55 , 02/08/2020 00:05:00 ]
            var lessThan = queryFromDate.AddSeconds(5);
            var greaterThan = queryFromDate.AddSeconds(-5);

            var conditions = new List<QueryFilterCondition>()
            {
                new QueryFilterCondition (string.Empty, t.GetProperty("Type").Name, QueryComparisons.Equal, ExtractTypeEnum.Archive.ToString()),
                new QueryFilterCondition ("datetime", t.GetProperty("QueryToDate").Name, QueryComparisons.GreaterThan, greaterThan),
                 new QueryFilterCondition ("datetime", t.GetProperty("QueryToDate").Name, QueryComparisons.LessThan, lessThan)
            };

            var tableQuery = QueryFilterUtil.AndQueryFilters<FulfillmentEntity>(conditions);

            var response = await Query(tableQuery);

            return response
                    ?.OrderByDescending(x => x.QueryFromDate)
                    ?.FirstOrDefault();
        }

        public async Task<FulfillmentEntity> GetLastSuccessfulFulfillment(string jobName)
        {
            var response = await QueryByPartition<FulfillmentEntity>(jobName);

            return response
                    ?.Where(x => x.CompleteSuccess == true)
                    ?.OrderByDescending(x => x.JobCompletedDate)
                    ?.FirstOrDefault() ?? null;
        }

        public async Task<List<FulfillmentEntity>> GetFailedFulfillments(string jobName)
        {
            var t = typeof(FulfillmentEntity);
            var conditions = new List<QueryFilterCondition>()
            {
                new QueryFilterCondition (string.Empty, t.GetProperty("JobName").Name, QueryComparisons.Equal, jobName),
                new QueryFilterCondition ("bool", t.GetProperty("CompleteSuccess").Name, QueryComparisons.Equal, false)
            };

            var tableQuery = QueryFilterUtil.AndQueryFilters<FulfillmentEntity>(conditions);

            var response = await Query(tableQuery);

            return response
                    ?.OrderByDescending(x => x.JobCompletedDate)
                    ?.ToList()
                    ?? new List<FulfillmentEntity>();
        }
    }
}
