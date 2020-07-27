using arx.Extract.Data.Common;
using arx.Extract.Data.Entities;
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
        Task<FulfillmentEntity> GetLastFulfillment(string jobName);
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

        public async  Task<List<FulfillmentEntity>> GetFulfillments(string jobName)
        {
            var response = await QueryByPartition<FulfillmentEntity>(jobName);

            return response?.ToList();
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

        public async Task<FulfillmentEntity> GetLastFulfillment(string jobName)
        {
            var response = await QueryByPartition<FulfillmentEntity>(jobName);

            return response
                    ?.OrderByDescending(x => x.JobCompletedDate)
                    ?.FirstOrDefault();
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
