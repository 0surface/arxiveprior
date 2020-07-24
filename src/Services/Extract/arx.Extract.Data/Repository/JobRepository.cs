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
        JobEntity GetJob(ExtractTypeEnum type, string jobName);
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

        public JobEntity GetJob(ExtractTypeEnum type,  string jobName)
        {
            var result = QueryByPartitionAndRow<JobEntity>(type.ToString(), jobName).Result;
            return result;
        }

        public bool SeedJobs()
        {
            IEnumerable<JobEntity> entities = SeedReader.ReadJobs();
            int resultCount = 0;
            //Insert each job one at a time.
            foreach (var job in entities)
            {
                var inserted = InsertOrReplace(job).Result.Result;
                if (inserted != null) resultCount++; ;
            }
            return resultCount == entities.Count();
        }

        public bool HasSeed() => HasAnyPartitionKey();
        public string TableName() => Name;
    }
}
