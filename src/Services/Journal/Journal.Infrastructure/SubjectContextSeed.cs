using Journal.Domain.AggregatesModel.SubjectAggregate;
using Journal.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Journal.Infrastructure
{
    /// <summary>
    /// For a comprehensive demonstartion of DbContext seeding desgin, construction and methodology
    /// Go to https://github.com/dotnet-architecture/eShopOnContainers via the Ordering Microservice API projects.
    /// </summary>
    public class SubjectContextSeed
    {
        public async Task SeedAsync(SubjectContext context, IOptions<JournalConfiguration> settings, ILogger<SubjectContextSeed> logger)
        {
            var policy = SeedExtensions.CreatePolicy(logger, nameof(SubjectContextSeed));

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
                string data = subjectFileResourceName.ReadDocumentFromExecutingAssembly();
                var entities = JsonConvert.DeserializeObject<IEnumerable<Subject>>(data);
                return entities;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
                throw new Exception("Error reading Subject Seed Json File", ex);
            }
        }
    }
}
