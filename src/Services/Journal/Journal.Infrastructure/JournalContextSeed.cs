using Journal.Domain.AggregatesModel.ArticleAggregate;
using Journal.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;

namespace Journal.Infrastructure
{
    /// <summary>
    /// For a comprehensive demonstartion of DbContext seeding desgin, construction and methodology
    /// Go to https://github.com/dotnet-architecture/eShopOnContainers via the Ordering Microservice API projects.
    /// </summary>
    public class JournalContextSeed
    {
        public async Task SeedAsync(JournalContext context, IOptions<JournalConfiguration> settings, ILogger<JournalContextSeed> logger)
        {
            var policy = SeedExtensions.CreatePolicy(logger, nameof(JournalContextSeed));

            await policy.ExecuteAsync(async () =>
            {
                using (context)
                {
                    context.Database.Migrate();

                    if (!context.Discipline.Any())
                    {
                        context.Discipline.AddRange(Discipline.GetAll<Discipline>());
                        await context.SaveChangesAsync();
                    }

                    if (!context.SubjectGroup.Any())
                    {
                        context.SubjectGroup.AddRange(SubjectGroup.GetAll<SubjectGroup>());
                        await context.SaveChangesAsync();
                    }

                    if (!context.SubjectCode.Any())
                    {
                        context.SubjectCode.AddRange(SubjectCode.GetAll<SubjectCode>());
                    }

                    await context.SaveChangesAsync();
                }
            });
        }
    }
}
