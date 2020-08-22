using Journal.Domain.AggregatesModel.ArticleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Journal.Infrastructure.EntityConfigurations
{
    class ArticleEntityTypeConfiguration
        : IEntityTypeConfiguration<Article>
    {
        public void Configure(EntityTypeBuilder<Article> articleConfiguration)
        {
            articleConfiguration.ToTable("Article");
            articleConfiguration.HasMany(p => p.CategoryArticles);
            articleConfiguration.HasMany(p => p.AuthorArticles);
            articleConfiguration.HasMany(p => p.PaperVersions)
                .WithOne(x => x.Article)
                .Metadata.PrincipalToDependent.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
