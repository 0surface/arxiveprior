using arx.Extract.Data.Common;
using arx.Extract.Data.Entities;
using System;
using System.Collections.Generic;

namespace arx.Extract.Data.Repository
{
    public interface IJobItemRepository
    {
        void SeedJobItems();       
        List<JobItemEntity> GetJobItems(Guid jobId);
    }

    public class JobItemRepository : TableStorage, IJobItemRepository
    {
        public JobItemRepository(string connectionString, string tableName) : base(tableName, connectionString)
        {
            Reference.CreateIfNotExists();
        }

        public List<JobItemEntity> GetJobItems(Guid jobId)
        {
            throw new NotImplementedException();
        }

        public void SeedJobItems()
        {
            throw new NotImplementedException();
        }
    }
}
