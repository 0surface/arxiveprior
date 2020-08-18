using Journal.Domain.AggregatesModel.ArticleAggregate;
using Microsoft.EntityFrameworkCore;

namespace Journal.Infrastructure
{
    public class ArticleContext : DbContext
    {
        public DbSet<Article> Articles { get; set; }

        public DbSet<Category> Categories { get; set; }

        public ArticleContext(DbContextOptions<ArticleContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>(x =>
            {
                x.ToTable("Article");               
                x.HasOne(p => p.PrimarySubjectCode);
                x.HasOne(p => p.PrimarySubjectGroupCode);
                x.HasOne(p => p.PrmiaryDiscipline);
                x.HasMany(p => p.CategoryArticles);
                x.HasMany(p => p.AuthorArticles);
                x.HasMany(p => p.PaperVersions).WithOne(x => x.Article)
                    .Metadata.PrincipalToDependent.SetPropertyAccessMode(PropertyAccessMode.Field);
            });

            modelBuilder.Entity<PaperVersion>(x => 
            {
                x.ToTable("Version").HasKey(k => k.Id);
                x.HasOne(p => p.Article).WithMany(p => p.PaperVersions);
            });

            modelBuilder.Entity<Category>(x => 
            {
                x.ToTable("Category").HasKey(k => k.Id);
                x.HasOne(p => p.SubjectCode).WithMany();
                x.HasOne(p => p.SubjectGroup).WithMany();
                x.HasOne(p => p.Discipline).WithMany();
            });

            modelBuilder.Entity<Author>(x =>
            {
                x.ToTable("Author").HasKey(k => k.Id);                
            });

            modelBuilder.Entity<CategoryArticle>()
                        .HasKey(k => new { k.CategoryId, k.ArticleId });

            modelBuilder.Entity<AuthorArticle>()
                        .HasKey( k => new {k.AuthorId, k.ArticleId });

            modelBuilder.Entity<AuthorAffiliation>()
                        .HasKey(k => new { k.AuthorId, k.AffiliationId });
        }
    }
}
