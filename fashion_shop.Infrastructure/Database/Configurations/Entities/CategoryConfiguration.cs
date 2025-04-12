using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.Entities;
using fashion_shop.Infrastructure.Database.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fashion_shop.Infrastructure.Database.EntityConfiguration;

public class CategoryConfiguration : ConfigurationEntity<Category>
{
    public override void Configure(EntityTypeBuilder<Category> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.Name)
               .HasMaxLength(255)
               .IsRequired();

        builder.HasIndex(p => p.Slug)
               .IsUnique();
    }
}
