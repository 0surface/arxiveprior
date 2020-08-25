using Journal.Domain.AggregatesModel.JobAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Journal.Infrastructure
{
    public class FulfillmentContext : DbContext
    {
        public DbSet<Fulfillment> Fulfillments { get; set; }

        public FulfillmentContext(DbContextOptions<FulfillmentContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }

    public class FulfillmentDbContextFactory : IDesignTimeDbContextFactory<FulfillmentContext>
    {
        public static readonly string EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        public FulfillmentContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                            .AddJsonFile($"appsettings.{EnvironmentName}.json", optional: false, reloadOnChange: true)
                            .AddEnvironmentVariables()
                            .Build();

            var optionsBuilder = new DbContextOptionsBuilder<FulfillmentContext>();

            optionsBuilder.UseSqlServer(config["ConnectionString"], sqlServerOptionsAction: o => o.MigrationsAssembly("Journal.Infrastructure"));

            return new FulfillmentContext(optionsBuilder.Options);
        }
    }
}
