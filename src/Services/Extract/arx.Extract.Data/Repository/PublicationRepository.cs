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

        Task<List<PublicationItemEntity>> GetByFulfilmentId(string fulfillmentId);
    }

    public class PublicationRepository : TableStorage, IPublicationRepository
    {
        public PublicationRepository(string connectionString, string tableName) : base(tableName, connectionString)
        {
            Reference.CreateIfNotExists();
        }

        public async Task<int> BatchSavePublications(List<PublicationItemEntity> publications)
        {
            try
            {
                var result = await Insert(publications);
                return result.Count();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public Task<List<PublicationItemEntity>> GetBetweenDates(DateTime fromDate, DateTime toDate)
        {
            throw new NotImplementedException();
        }

        public async Task<List<PublicationItemEntity>> GetSubjectInclusiveBetweenDates(string subjectCode, DateTime updatedFromDate, DateTime updatedToDate)
        {
            var t = typeof(PublicationItemEntity);
            var conditions = new List<QueryFilterCondition>()
            {
                new QueryFilterCondition(string.Empty, t.GetProperty("PrimarySubjectCode").Name, QueryComparisons.Equal, subjectCode),
                new QueryFilterCondition("datetime", t.GetProperty("UpdatedDate").Name, QueryComparisons.GreaterThanOrEqual, updatedFromDate),
                new QueryFilterCondition("datetime", t.GetProperty("UpdatedDate").Name, QueryComparisons.LessThanOrEqual, updatedToDate),
            };

            var tableQuery = QueryFilterUtil.AndQueryFilters<PublicationItemEntity>(conditions);

            var response = await Query(tableQuery);

            return response
                    ?.OrderByDescending(x => x.UpdatedDate)
                    ?.ThenBy(x => x.PrimarySubjectCode)
                    ?.ToList()
                    ?? new List<PublicationItemEntity>();
        }

        public Task<List<PublicationItemEntity>> GetSubjectGroupBetweenDates(string subjectGroupCode, DateTime fromDate, DateTime toDate)
        {
            throw new NotImplementedException();
        }

        public async Task<List<PublicationItemEntity>> GetByFulfilmentId(string fulfillmentId)
        {
            var response = await QueryByPartition<PublicationItemEntity>(fulfillmentId);

            return response
                   ?.OrderByDescending(x => x.UpdatedDate)
                   ?.ThenBy(x => x.PrimarySubjectCode)
                   ?.ToList()
                   ?? new List<PublicationItemEntity>();
        }
    }
}

