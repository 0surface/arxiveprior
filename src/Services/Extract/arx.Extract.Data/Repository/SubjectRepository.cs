using arx.Extract.Data.Common;
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
        Task<IEnumerable<SubjectEntity>> GetAll();
        Task<IEnumerable<SubjectEntity>> GetSubjectsByGroupCode(string groupCode);
        Task<IEnumerable<SubjectEntity>> GetSubjectsByCode(string code);
        bool Seed();
    }
    public class SubjectRepository : TableStorage, ISubjectRepository
    {
        public SubjectRepository(string connectionString, string tableName) : base(tableName, connectionString)
        {
            Reference.CreateIfNotExists();
        }

        public async Task<IEnumerable<SubjectEntity>> GetAll()
            => await Query<SubjectEntity>(new TableQuery<SubjectEntity>());

        public async Task<IEnumerable<SubjectEntity>> GetSubjectsByCode(string code)
            => await QueryByRow<SubjectEntity>(code);

        public async Task<IEnumerable<SubjectEntity>> GetSubjectsByGroupCode(string groupCode)
        {
            var conditions = new List<QueryFilterCondition>()
            { new QueryFilterCondition(string.Empty, "GroupCode", QueryComparisons.Equal, groupCode) };

            var tableQuery = QueryFilterUtil.AndQueryFilters<SubjectEntity>(conditions);

            return await Query(tableQuery);
        }

        public bool Seed()
        {
            var entities = SeedReader.ReadSubjects();
            var batchOperation = new TableBatchOperation();
            entities.ToList().ForEach(e => batchOperation.InsertOrReplace(e));
            var result = BatchInsertExtensions.ExecuteBatchAsLimitedBatches(Reference, batchOperation, null);

            return result.Count == entities.Count();
        }
    }
}
