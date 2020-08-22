using Journal.Domain.AggregatesModel.ArticleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Journal.Infrastructure.EntityConfigurations
{
    class AuthorAffliationEntityTypeConfiguration
        : IEntityTypeConfiguration<AuthorAffiliation>
    {
        public void Configure(EntityTypeBuilder<AuthorAffiliation> authorAffilationConfigration)
        {
            authorAffilationConfigration.HasKey(k => new { k.AuthorId, k.AffiliationId });
        }
    }
}
