using Journal.Domain.AggregatesModel.ArticleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Journal.Infrastructure.EntityConfigurations
{
    class AuthorArticleEntityTypeConfiguration
            : IEntityTypeConfiguration<AuthorArticle>
    {
        public void Configure(EntityTypeBuilder<AuthorArticle> authorArticleConfigration)
        {
            authorArticleConfigration.HasKey(k => new { k.AuthorId, k.ArticleId });
        }
    }
}
