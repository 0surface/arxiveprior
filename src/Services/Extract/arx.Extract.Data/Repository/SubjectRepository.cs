using arx.Extract.Data.Entities;
using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace arx.Extract.Data.Repository
{
    public class SubjectRepository
    {
        private CloudTable subjectTable = null;
        public SubjectRepository(string connectionString, string table)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            subjectTable = tableClient.GetTableReference("Subjects");
            subjectTable.CreateIfNotExists();
        }

        public async Task<IEnumerable<SubjectEntity>> All()
        {
            var query = new TableQuery<SubjectEntity>();

            var entities = subjectTable.ExecuteQuery(query);

            return await Task.FromResult(entities);
        }

        public async void Seed()
        {
            await Task.Run(() => { });
        }
    }
}
