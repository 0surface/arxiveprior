using arx.Extract.Data.Common;
using arx.Extract.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace arx.Extract.Data.Repository
{
    public interface IPublicationRepository 
    {
        Task<int> BatchSavePublications(List<PublicationItemEntity> publications);
        Task<List<PublicationItemEntity>> GetBetweenDates(DateTime fromDate, DateTime toDate);
        Task<List<PublicationItemEntity>> GetSubjectBetweenDates(string subjectCode, DateTime fromDate, DateTime toDate);
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
            throw new System.NotImplementedException();
        }

        public Task<List<PublicationItemEntity>> GetBetweenDates(DateTime fromDate, DateTime toDate)
        {
            throw new NotImplementedException();
        }

        public Task<List<PublicationItemEntity>> GetSubjectBetweenDates(string subjectCode, DateTime fromDate, DateTime toDate)
        {
            throw new NotImplementedException();
        }

        public Task<List<PublicationItemEntity>> GetSubjectGroupBetweenDates(string subjectGroupCode, DateTime fromDate, DateTime toDate)
        {
            throw new NotImplementedException();
        }
    }
}
