using arx.Extract.Data.Entities;
using arx.Extract.Data.Extensions;
using arx.Extract.Data.Seed;
using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace arx.Extract.Data.Repository
{
    public interface ISubjectRepository
    {
        Task<IEnumerable<SubjectEntity>> All();
        bool Seed();
    }
    public class SubjectRepository : ISubjectRepository
    {
        private readonly string _connectionString;
        private readonly string _tableName;
        private CloudTable subjectTable = null;
        public SubjectRepository()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            subjectTable = tableClient.GetTableReference(_tableName);
            subjectTable.CreateIfNotExists();
        }
        public SubjectRepository(string connectionString, string tableName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            subjectTable = tableClient.GetTableReference(tableName);
            subjectTable.CreateIfNotExists();
            _connectionString = connectionString;
            _tableName = tableName;
        }

        public async Task<IEnumerable<SubjectEntity>> All()
        {
            var query = new TableQuery<SubjectEntity>();

            var entities = subjectTable.ExecuteQuery(query);

            return await Task.FromResult(entities);
        }

        public bool Seed()
        {
            var entities = SeedReader.ReadSubjects();
            var batchOperation = new TableBatchOperation();
            entities.ToList().ForEach(e => batchOperation.InsertOrReplace(e));
            var result = BatchInsertExtensions.ExecuteBatchAsLimitedBatches(subjectTable, batchOperation, null);

            return result.Count == entities.Count();
        }
    }
}
