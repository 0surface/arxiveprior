using arx.Extract.Data.Common;
using arx.Extract.Data.Entities;
using arx.Extract.Data.Extensions;
using arx.Extract.Data.Seed;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
            => await Query<SubjectEntity>(FilteredQuery("GroupCode", groupCode));
        
        public bool Seed()
        {
            var entities = SeedReader.ReadSubjects();
            var batchOperation = new TableBatchOperation();
            entities.ToList().ForEach(e => batchOperation.InsertOrReplace(e));
            var result = BatchInsertExtensions.ExecuteBatchAsLimitedBatches(Reference, batchOperation, null);

            return result.Count == entities.Count();
        }

        private TableQuery<SubjectEntity> FilteredQuery(string propertyName, string filterValue)
        {
            return new TableQuery<SubjectEntity>()
                        .Where(TableQuery.GenerateFilterCondition(
                                            typeof(SubjectEntity).GetProperty(propertyName).Name,
                                            QueryComparisons.Equal,
                                            filterValue));
        }
    }
}
