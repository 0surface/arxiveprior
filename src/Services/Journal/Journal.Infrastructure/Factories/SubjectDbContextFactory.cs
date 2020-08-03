using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Journal.Infrastructure.Factories
{
    public class SubjectDbContextFactory : IDesignTimeDbContextFactory<SubjectContext>
    {
        public static readonly string EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        public SubjectContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                            .AddJsonFile($"appsettings.{EnvironmentName}.json", optional: false, reloadOnChange: true)
                            .AddEnvironmentVariables()
                            .Build();

            var optionsBuilder = new DbContextOptionsBuilder<SubjectContext>();

            optionsBuilder.UseSqlServer(config["ConnectionString"], sqlServerOptionsAction: o => o.MigrationsAssembly("Journal.Infrastructure"));

            return new SubjectContext(optionsBuilder.Options);
        }
    }
}
