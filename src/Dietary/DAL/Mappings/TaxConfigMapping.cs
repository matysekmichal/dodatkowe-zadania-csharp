﻿using LegacyFighter.Dietary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacyFighter.Dietary.DAL.Mappings
{
    public class TaxConfigMapping : IEntityTypeConfiguration<TaxConfig>
    {
        public void Configure(EntityTypeBuilder<TaxConfig> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasMany(x => x.TaxRules);
            builder.Property(x => x.CountryCode)
                .HasConversion(x => x.Value, x => CountryCode.Of(x));
        }
    }
}