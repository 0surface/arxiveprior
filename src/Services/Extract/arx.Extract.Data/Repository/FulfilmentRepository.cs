using arx.Extract.Data.Common;
using arx.Extract.Data.Entities;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace arx.Extract.Data.Repository
{
    public interface IFulfilmentRepository
    {
        FulfilmentEntity GetLastFulfilment(string jobName);
        List<FulfilmentItemEntity> GetFulfilmentItems(string JobRecordId);
        Task<FulfilmentEntity> SaveFulfilment(FulfilmentEntity jobRecord);

    }
    public class FulfilmentRepository : TableStorage, IFulfilmentRepository
    {
        public FulfilmentRepository(string connectionString, string tableName) : base(tableName, connectionString)
        {
            Reference.CreateIfNotExists();
        }

        public List<FulfilmentItemEntity> GetFulfilmentItems(string JobRecordId)
        {
            throw new NotImplementedException();
        }

        public FulfilmentEntity GetLastFulfilment(string jobName)
        {
            var conditions = new List<QueryFilterCondition>()
            {
                new QueryFilterCondition(string.Empty, "PartitionKey", QueryComparisons.Equal, jobName)
            };

            var tableQuery = QueryFilterUtil.AndQueryFilters<FulfilmentEntity>(conditions);

            return Query(tableQuery)
                        .Result
                        .OrderByDescending(x => x.JobCompletedDate)
                        .FirstOrDefault();
        }

        public async Task<FulfilmentEntity> SaveFulfilment(FulfilmentEntity fulfilment)
        {
            try
            {
                var response = await InsertOrReplace(fulfilment);
                return (FulfilmentEntity)response.Result;
            }
            catch (Exception)
            {
                return null;
            }
        }


    }
}
