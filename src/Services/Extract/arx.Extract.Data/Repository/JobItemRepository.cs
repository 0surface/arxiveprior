using arx.Extract.Data.Common;
using arx.Extract.Data.Entities;
using arx.Extract.Data.Extensions;
using arx.Extract.Data.Seed;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

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
            throw new NotImplementedException();
        }

                

        public bool SeedJobItems()
        {
            int resultCount = 0;
            var entities = SeedReader.ReadJobItems();                        
            var partitionKeys = entities.Select(x => x.PartitionKey).Distinct();

            //Insert each jobItem Collection as a batch by Partition Key
            foreach (var key in partitionKeys)
            {
                var batchOperation = new TableBatchOperation();

                entities
                    .Where(j => j.PartitionKey == key)
                    .ToList()                    
                    .ForEach(e => batchOperation.InsertOrReplace(e));

                var inserted = BatchInsertExtensions.ExecuteBatchAsLimitedBatches(Reference, batchOperation, null);

                if (inserted != null) resultCount++;;
            }
            return resultCount == entities.Count();
        }
        public bool HasSeed() => HasAnyPartitionKey();
        public string TableName() => Name;
    }
}
