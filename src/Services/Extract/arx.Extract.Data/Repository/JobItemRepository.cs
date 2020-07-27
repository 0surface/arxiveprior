using arx.Extract.Data.Common;
using arx.Extract.Data.Entities;
using arx.Extract.Data.Extensions;
using arx.Extract.Data.Seed;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace arx.Extract.Data.Repository
{
    public interface IJobItemRepository
    {
        List<JobItemEntity> GetJobItems(string jobName);
        bool HasSeed();
        bool SeedJobItems();
        string TableName();
    }

    public class JobItemRepository : TableStorage, IJobItemRepository
    {
        public JobItemRepository(string connectionString, string tableName) : base(tableName, connectionString)
        {
            Reference.CreateIfNotExists();
        }

        public List<JobItemEntity> GetJobItems(string jobName)
        {
            var jobItems = QueryByPartition<JobItemEntity>(jobName).Result;
            return jobItems?.ToList() ?? new List<JobItemEntity>();
        }

        public bool SeedJobItems()
        {
            IEnumerable<JobItemEntity> entities = SeedReader.ReadJobItems();
                        
            bool batchSuccess = true;

            //Insert each jobItem Collection as a batch segemented by Partition Key
            foreach (var key in entities.Select(x => x.PartitionKey).Distinct())
            {
                var batchOperation = new TableBatchOperation();

                entities
                    .Where(j => j.PartitionKey == key)
                    .ToList()
                    .ForEach(e => batchOperation.InsertOrReplace(e));

                var inserted = BatchInsertExtensions.ExecuteBatchAsLimitedBatches(Reference, batchOperation, null);

                if (batchSuccess)
                {
                    batchSuccess = inserted?.All(r => r.HttpStatusCode < 400) ?? false;
                }
            }

            return batchSuccess;
        }
        public bool HasSeed() => HasAnyPartitionKey();
        public string TableName() => Name;
    }
}
