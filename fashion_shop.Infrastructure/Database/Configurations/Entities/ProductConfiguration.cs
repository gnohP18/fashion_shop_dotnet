using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fashion_shop.Infrastructure.Database.Configurations.Entities;

public class ProductConfiguration : ConfigurationEntity<Product>
{

    public override void Configure(EntityTypeBuilder<Product> builder)
    {
        base.Configure(builder);

        builder.ToTable("products");

        builder.Property(p => p.Name)
               .HasMaxLength(255)
               .IsRequired();

        builder.Property(p => p.Slug)
               .HasMaxLength(400)
               .IsRequired();

        builder.HasIndex(p => p.Slug)
               .IsUnique();

        builder.Property(p => p.Price)
               .IsRequired();

        builder.Property(p => p.ImageUrl)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(p => p.Description)
               .HasMaxLength(1000)
               .IsRequired(false);

        builder.Property(p => p.IsVariant)
               .HasDefaultValue(false);

        builder.Property(p => p.CategoryId)
               .IsRequired();

        builder.HasOne(p => p.Category)
               .WithMany(c => c.Products)
               .HasForeignKey(p => p.CategoryId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
