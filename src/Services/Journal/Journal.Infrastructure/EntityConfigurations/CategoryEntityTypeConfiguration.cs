using Journal.Domain.AggregatesModel.ArticleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Journal.Infrastructure.EntityConfigurations
{
    class CategoryEntityTypeConfiguration
        : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> categoryConfiguration)
        {
            categoryConfiguration.ToTable("Category").HasKey(k => k.Id);
            categoryConfiguration.HasOne(p => p.SubjectCode).WithMany();
            categoryConfiguration.HasOne(p => p.SubjectGroup).WithMany();
            categoryConfiguration.HasOne(p => p.Discipline).WithMany();
        }
    }
}
