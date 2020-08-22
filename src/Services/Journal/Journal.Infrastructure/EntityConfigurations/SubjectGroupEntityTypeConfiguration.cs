using Journal.Domain.AggregatesModel.ArticleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Journal.Infrastructure.EntityConfigurations
{
    class SubjectGroupEntityTypeConfiguration
        : IEntityTypeConfiguration<SubjectGroup>
    {
        public void Configure(EntityTypeBuilder<SubjectGroup> subjectGroupConfiguration)
        {
            subjectGroupConfiguration.Property(p => p.Id);
            subjectGroupConfiguration.Property(p => p.Name);
            subjectGroupConfiguration.Property(p => p.DisciplineName);
        }
    }
}
