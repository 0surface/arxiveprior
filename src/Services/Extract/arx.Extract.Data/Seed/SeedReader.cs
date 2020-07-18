using arx.Extract.Data.Entities;
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
