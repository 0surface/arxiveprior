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
        FulfillmentEntity GetLastFulfillment(string jobName);
        List<FulfillmentItemEntity> GetFulfillmentItems(string JobRecordId);
        Task<FulfillmentEntity> SaveFulfillment(FulfillmentEntity jobRecord);

    }
    public class FulfillmentRepository : TableStorage, IFulfillmentRepository
    {
        public FulfillmentRepository(string connectionString, string tableName) : base(tableName, connectionString)
        {
            Reference.CreateIfNotExists();
        }

        public List<FulfillmentItemEntity> GetFulfillmentItems(string JobRecordId)
        {
            throw new NotImplementedException();
        }

        public FulfillmentEntity GetLastFulfillment(string jobName)
        {
            var conditions = new List<QueryFilterCondition>()
            {
                new QueryFilterCondition(string.Empty, "PartitionKey", QueryComparisons.Equal, jobName)
            };

            var tableQuery = QueryFilterUtil.AndQueryFilters<FulfillmentEntity>(conditions);

            return Query(tableQuery)
                        .Result
                        .OrderByDescending(x => x.JobCompletedDate)
                        .FirstOrDefault();
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


    }
}
