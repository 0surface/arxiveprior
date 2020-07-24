using arx.Extract.Data.Common;
using arx.Extract.Data.Entities;
using arx.Extract.Data.Extensions;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace arx.Extract.Data.Repository
{
    public interface IPublicationRepository
    {
        Task<int> BatchSavePublications(List<PublicationItemEntity> publications);
        Task<List<PublicationItemEntity>> GetBetweenDates(DateTime fromDate, DateTime toDate);
        Task<List<PublicationItemEntity>> GetSubjectInclusiveBetweenDates(string subjectCode, DateTime fromDate, DateTime toDate);
        Task<List<PublicationItemEntity>> GetSubjectGroupBetweenDates(string subjectGroupCode, DateTime fromDate, DateTime toDate);
    }

    public class PublicationRepository : TableStorage, IPublicationRepository
    {
        public PublicationRepository(string connectionString, string tableName) : base(tableName, connectionString)
        {
            Reference.CreateIfNotExists();
        }

        public Task<int> BatchSavePublications(List<PublicationItemEntity> publications)
        {
            int resultCount = 0;
            var partitionKeys = publications.Select(x => x.PartitionKey).Distinct().ToList();

            foreach (var key in partitionKeys)
            {
                var batchOperation = new TableBatchOperation();

                publications
                    .Where(j => j.PartitionKey == key)
                    .ToList()
                    .ForEach(e => batchOperation.InsertOrReplace(e));

                var inserted = BatchInsertExtensions.ExecuteBatchAsLimitedBatches(Reference, batchOperation, null);

                if (inserted != null) resultCount++;
            }
            return Task.FromResult(resultCount);
        }

        public Task<List<PublicationItemEntity>> GetBetweenDates(DateTime fromDate, DateTime toDate)
        {
            throw new NotImplementedException();
        }

        public async Task<List<PublicationItemEntity>> GetSubjectInclusiveBetweenDates(string subjectCode, DateTime updatedFromDate, DateTime updatedToDate)
        {
            var conditions = new List<QueryFilterCondition>()
            {
                new QueryFilterCondition(string.Empty, "PartitionKey", QueryComparisons.Equal, subjectCode),
                new QueryFilterCondition("datetime", "UpdatedDate", QueryComparisons.GreaterThanOrEqual, updatedFromDate),
                new QueryFilterCondition("datetime", "UpdatedDate", QueryComparisons.LessThanOrEqual, updatedToDate),
            };

            var tableQuery = QueryFilterUtil.AndQueryFilters<PublicationItemEntity>(conditions);

            var response = await Query(tableQuery);

            return response
                    ?.OrderByDescending(x => x.UpdatedDate)
                    ?.ToList()
                    ?? new List<PublicationItemEntity>();
        }

        public Task<List<PublicationItemEntity>> GetSubjectGroupBetweenDates(string subjectGroupCode, DateTime fromDate, DateTime toDate)
        {
            throw new NotImplementedException();
        }
    }
}

