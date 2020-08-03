using Journal.Domain.AggregatesModel.SubjectAggregate;
using Microsoft.EntityFrameworkCore;

namespace Journal.Infrastructure
{
    public class SubjectContext : DbContext
    {
        public const string DEFAULT_SCHEMA = "journal";

        public DbSet<Subject> Subjects { get; set; }

        public SubjectContext(DbContextOptions<SubjectContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

    }
}
