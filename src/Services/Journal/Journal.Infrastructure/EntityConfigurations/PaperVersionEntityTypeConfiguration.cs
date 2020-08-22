using Journal.Domain.AggregatesModel.ArticleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Journal.Infrastructure.EntityConfigurations
{
    class PaperVersionEntityTypeConfiguration
        : IEntityTypeConfiguration<PaperVersion>
    {
        public void Configure(EntityTypeBuilder<PaperVersion> paperVersionConfiguration)
        {
            paperVersionConfiguration.ToTable("Version");

            paperVersionConfiguration.HasKey(k => k.Id);
            
            paperVersionConfiguration
                .HasOne(p => p.Article)
                .WithMany(a => a.PaperVersions);
        }
    }
}
