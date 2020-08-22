using Journal.Domain.AggregatesModel.ArticleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journal.Infrastructure.EntityConfigurations
{
    class AffiliationEntityTypeConfiguration
        :IEntityTypeConfiguration<Affiliation>
    {
        public void Configure(EntityTypeBuilder<Affiliation> affliationConfiguration)
        {
            affliationConfiguration.ToTable("Affiliation");
            affliationConfiguration.HasMany(a => a.AuthorAffiliations);
        }
    }
}
