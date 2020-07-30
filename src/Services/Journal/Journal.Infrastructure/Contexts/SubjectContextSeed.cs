using Common.Util.ReadWrite;
using Journal.Domain.AggregatesModel.SubjectAggregate;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Journal.Infrastructure.Contexts
{
    /// <summary>
    /// For a comprehensive demonstartion of DbContext seeding desgin, construction and methodology
    /// Go to https://github.com/dotnet-architecture/eShopOnContainers via the Ordering Microservice API projects.
    /// </summary>
    public class SubjectContextSeed
    {
        public async Task SeedAsync(SubjectContext context, IOptions<JournalInfrastructureSettings> settings, ILogger<SubjectContextSeed> logger)
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
                string data = ReadUtil.ReadDocumentFromExecutingAssembly(subjectFileResourceName);
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
    }
}
