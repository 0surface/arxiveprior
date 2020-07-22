using arx.Extract.Data.Entities;
using arx.Extract.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace arx.Extract.Data.Seed
{
    public class SeedReader
    {
        /// <summary>
        /// Reads the seed json file, assign partition and row key values to the table entity type.
        /// </summary>
        /// <returns>IEnumerable<SubjectEntity></returns>
        public static IEnumerable<SubjectEntity> ReadSubjects()
        {
            string subjectFileResourceName = "arx.Extract.Data.Seed.SubjectSeedData.json";
            string data = ReadDocument(subjectFileResourceName);
            var entities =  JsonConvert.DeserializeObject<IEnumerable<SubjectEntity>>(data);

            foreach (var subject in entities)
            {
                subject.PartitionKey = "arxiv";
                subject.RowKey = subject.Code;
                subject.Timestamp = DateTime.UtcNow;
            }
            return entities;
        }

        public static IEnumerable<JobEntity> ReadJobs()
        {
            string jobsFileResourceName = "arx.Extract.Data.Seed.JobSeedData.json";
            string data = ReadDocument(jobsFileResourceName);
            var entities = JsonConvert.DeserializeObject<IEnumerable<JobEntity>>(data);

            foreach (var job in entities)
            {
                job.PartitionKey = job.Type.ToString();
                job.RowKey = job.UniqueName;
                job.Timestamp = DateTime.UtcNow;
            }
            return entities;
        }

        public static IEnumerable<JobItemEntity> ReadJobItems()
        {
            string jobItemsFileResourceName = "arx.Extract.Data.Seed.JobItemSeedData.json";
            string data = ReadDocument(jobItemsFileResourceName);
            var entities = JsonConvert.DeserializeObject<IEnumerable<JobItemEntity>>(data);

            foreach (var item in entities)
            {                
                item.JobItemId = Guid.NewGuid();

                item.PartitionKey = item.JobName;
                item.RowKey = item.JobItemId.ToString();
                item.Timestamp = DateTime.UtcNow;
            }
            return entities;
        }


        private static string ReadDocument(string resourceName)
        {
            string data = "";
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                data = reader.ReadToEnd();
            }

            return data;
        }
    }
}
