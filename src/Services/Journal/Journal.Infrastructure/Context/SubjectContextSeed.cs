using Journal.Domain.AggregatesModel.SubjectAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Journal.Infrastructure
{
    /// <summary>
    /// For a comprehensive demonstartion of DbContext seeding desgin, construction and methodology
    /// Go to https://github.com/dotnet-architecture/eShopOnContainers via the Ordering Microservice API projects.
    /// </summary>
    public class SubjectContextSeed
    {
        public async Task SeedAsync(SubjectContext context, IOptions<JournalSettings> settings, ILogger<SubjectContextSeed> logger)
        {
            var policy = CreatePolicy(logger, nameof(SubjectContextSeed));

            await policy.ExecuteAsync(async () =>
            {
                using (context)
                {
                    context.Database.Migrate();

                    if (!context.Subjects.Any())
                    {
                        context.Subjects.AddRange(ReadSubjects(logger));
                    }

                    await context.SaveChangesAsync();
                }
            });
        }

        private static IEnumerable<Subject> ReadSubjects(ILogger<SubjectContextSeed> logger)
        {
            try
            {
                string subjectFileResourceName = "Journal.Infrastructure.Setup.SubjectSeedData.json";
                string data = ReadDocumentFromExecutingAssembly(subjectFileResourceName);
                var entities = JsonConvert.DeserializeObject<IEnumerable<Subject>>(data);
                return entities;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
                throw new Exception("Error reading Subject Seed Json File", ex);
            }
        }

        private AsyncRetryPolicy CreatePolicy(ILogger<SubjectContextSeed> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
                    }
                );
        }

        public static string ReadDocumentFromExecutingAssembly(string resourceName)
        {
            string data = "";
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                using Stream stream = assembly.GetManifestResourceStream(resourceName);
                using StreamReader reader = new StreamReader(stream);
                data = reader.ReadToEnd();
            }
            catch (Exception)
            {
            }

            return data;
        }
    }
}
