using Journal.Domain.AggregatesModel.SubjectAggregate;
using Microsoft.EntityFrameworkCore;

namespace Journal.API.Infrastructure
{
    public class JournalContext : DbContext
    {
        public const string DEFAULT_SCHEMA = "journal";

        public DbSet<Subject> Subjects { get; set; }

        public JournalContext(DbContextOptions<JournalContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

    }
}
