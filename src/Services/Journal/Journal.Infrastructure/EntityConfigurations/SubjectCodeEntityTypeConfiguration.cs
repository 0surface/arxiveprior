using Journal.Domain.AggregatesModel.ArticleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Journal.Infrastructure.EntityConfigurations
{
    class SubjectCodeEntityTypeConfiguration 
        : IEntityTypeConfiguration<SubjectCode>
    {
        public void Configure(EntityTypeBuilder<SubjectCode> subjectCodeConfiguration)
        {
            subjectCodeConfiguration.Property(p => p.Id);
            subjectCodeConfiguration.Property(p => p.Name);
            subjectCodeConfiguration.Property(p => p.Description);
            subjectCodeConfiguration.Property(p => p.SubjectGroupCode);
        }
    }
}
