using arx.Extract.Data.Common;
using arx.Extract.Data.Entities;
using arx.Extract.Data.Extensions;
using arx.Extract.Data.Seed;
using arx.Extract.Types;
using Microsoft.Azure.Cosmos.Table;
using System.Linq;
using System.Collections.Generic;

namespace arx.Extract.Data.Repository
{
    public interface IJobRepository
    {       
        JobEntity GetJobByExtractonMode(string jobName);
        bool SeedJobs();
        bool HasSeed();
        string TableName();
    }

    public class JobRepository : TableStorage, IJobRepository
    {
        public JobRepository(string connectionString, string tableName) : base(tableName, connectionString)
        {
            Reference.CreateIfNotExists();
        }

        public JobEntity GetJobByExtractonMode(string jobName)
        {
            throw new System.NotImplementedException();
        }

        public bool SeedJobs()
        {   
            var entities = SeedReader.ReadJobs();
            var batchOperation = new TableBatchOperation();
            entities.ToList().ForEach(e => batchOperation.InsertOrReplace(e));
            var result = BatchInsertExtensions.ExecuteBatchAsLimitedBatches(Reference, batchOperation, null);

            return result.Count == entities.Count();
        }

        public bool HasSeed()
        {
            if (TableExists().Result == false)
                return false;

            return PartitionExists(ExtractTypeEnum.Archive.ToString())
                 && PartitionExists(ExtractTypeEnum.Journal.ToString())
                 && PartitionExists(ExtractTypeEnum.Seed.ToString());
        }
        public string TableName() => Name;
    }
}
