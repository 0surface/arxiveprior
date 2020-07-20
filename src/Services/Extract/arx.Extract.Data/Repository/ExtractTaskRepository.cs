using arx.Extract.Data.Common;

namespace arx.Extract.Data.Repository
{
    public interface IExtractTaskRepository
    {
        void SeedExtractTasks();
    }

    public class ExtractTaskRepository : TableStorage, IExtractTaskRepository
    {
        public ExtractTaskRepository(string connectionString, string tableName) : base(tableName, connectionString)
        {
            Reference.CreateIfNotExists();
        }

        public void SeedExtractTasks()
        {

        }

    }
}
