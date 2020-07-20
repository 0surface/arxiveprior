using arx.Extract.Data.Common;
using arx.Extract.Data.Entities;

namespace arx.Extract.Data.Repository
{
    public interface IJobRepository
    {
        void SeedJobs();
        JobEntity GetJobByExtractonMode(string jobName);
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

        public void SeedJobs()
        {
            throw new System.NotImplementedException();
        }
    }
}
