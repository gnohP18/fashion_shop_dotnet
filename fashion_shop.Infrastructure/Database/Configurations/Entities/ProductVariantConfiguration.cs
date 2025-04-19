using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fashion_shop.Infrastructure.Database.Configurations.Entities;

public class ProductVariantConfiguration : ConfigurationEntity<ProductVariant>
{
    public override void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        base.Configure(builder);

        builder.ToTable("product_variants");

        builder.Property(p => p.Name)
           .HasMaxLength(20)
           .IsRequired();

        builder.HasIndex(p => p.Priority);

        builder.HasOne(p => p.Product)
            .WithMany(p => p.ProductVariants)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}