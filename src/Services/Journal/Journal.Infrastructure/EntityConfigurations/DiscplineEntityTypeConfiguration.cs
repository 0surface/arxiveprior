using Journal.Domain.AggregatesModel.ArticleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Journal.Infrastructure.EntityConfigurations
{
    class DiscplineEntityTypeConfiguration
        : IEntityTypeConfiguration<Discipline>
    {
        public void Configure(EntityTypeBuilder<Discipline> discplineConfiguration)
        {
            discplineConfiguration.Property(d => d.Id);
            discplineConfiguration.Property(d => d.Name);
        }
    }
}
