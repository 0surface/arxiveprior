using Journal.Domain.AggregatesModel.ArticleAggregate;
using Journal.Domain.AggregatesModel.JobAggregate;
using Journal.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Journal.Infrastructure
{
    public class JournalContext : DbContext
    {
        public const string DEFAULT_SCHEMA = "journal";
        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<PaperVersion> PaperVersions { get; set; }
        public DbSet<Fulfillment> Fulfillments { get; set; }
        public DbSet<SubjectCode> SubjectCode { get; set; }
        public DbSet<SubjectGroup> SubjectGroup { get; set; }
        public DbSet<Discipline> Discipline { get; set; }

        public JournalContext(DbContextOptions<JournalContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AffiliationEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ArticleEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AuthorAffliationEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AuthorArticleEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AuthorEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryArticleEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DiscplineEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PaperVersionEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SubjectCodeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SubjectGroupEntityTypeConfiguration());
        }
    }

    public class JournalContextDesignFactory : IDesignTimeDbContextFactory<JournalContext>
    {
        public static readonly string EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        public JournalContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                            .AddJsonFile($"appsettings.{EnvironmentName}.json", optional: false, reloadOnChange: true)
                            .AddEnvironmentVariables()
                            .Build();

            var optionsBuilder = new DbContextOptionsBuilder<JournalContext>()
               .UseSqlServer(config["ConnectionString"], sqlServerOptionsAction: o => o.MigrationsAssembly("Journal.Infrastructure"));

            return new JournalContext(optionsBuilder.Options);
        }
    }
}
