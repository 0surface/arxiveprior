using Journal.Domain.AggregatesModel.ArticleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Journal.Infrastructure.EntityConfigurations
{
    class AuthorEntityTypeConfiguration
        : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> authorConfiguration)
        {
            authorConfiguration.ToTable("Author").HasKey(k => k.Id);
            authorConfiguration.HasMany(a => a.AuthorAffiliations);
        }
    }
}
