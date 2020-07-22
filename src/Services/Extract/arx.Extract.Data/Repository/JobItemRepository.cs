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
            throw new NotImplementedException();
        }

                

        public bool SeedJobItems()
        {
            var entities = SeedReader.ReadJobItems();
            var batchOperation = new TableBatchOperation();
            entities.ToList().ForEach(e => batchOperation.InsertOrReplace(e));
            var result = BatchInsertExtensions.ExecuteBatchAsLimitedBatches(Reference, batchOperation, null);            
            return  result.Count == entities.Count();
        }
        public bool HasSeed() => HasAnyPartitionKey();
        public string TableName() => Name;
    }
}
