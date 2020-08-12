using Journal.Domain.AggregatesModel.ArticleAggregate;
using Microsoft.EntityFrameworkCore;

namespace Journal.Infrastructure
{
    public class ArticleContext : DbContext
    {
        public DbSet<Article> Articles { get; set; }

        public ArticleContext(DbContextOptions<ArticleContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>(x =>
            {
                x.ToTable("Article");
                x.HasMany(p => p.PaperVersions).WithOne(x => x.Article)
                .Metadata.PrincipalToDependent.SetPropertyAccessMode(PropertyAccessMode.Field);
            });

            modelBuilder.Entity<PaperVersion>(x => 
            {
                x.ToTable("Version").HasKey(k => k.Id);
                x.HasOne(p => p.Article).WithMany(p => p.PaperVersions);
            });
        }
    }
}
