using Journal.Domain.AggregatesModel.ArticleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Journal.Infrastructure.EntityConfigurations
{
    class CategoryArticleEntityTypeConfiguration
        : IEntityTypeConfiguration<CategoryArticle>
    {
        public void Configure(EntityTypeBuilder<CategoryArticle> categoryArticleConfigration)
        {
            categoryArticleConfigration.HasKey(k => new { k.CategoryId, k.ArticleId });
        }
    }
}
