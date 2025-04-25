using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using fashion_shop.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fashion_shop.Infrastructure.Database.Configurations.Entities;

public class ProductItemConfiguration : ConfigurationEntity<ProductItem>
{
    public override void Configure(EntityTypeBuilder<ProductItem> builder)
    {
        base.Configure(builder);

        builder.ToTable("product_items");

        builder.Property(p => p.Code)
           .HasMaxLength(64)
           .IsRequired();

        builder.Property(p => p.ImageUrl);

        builder.Property(p => p.Quantity);

        builder.Property(p => p.Price);

        builder.HasOne(p => p.Product)
            .WithMany(p => p.ProductItems)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.OrderDetails)
               .WithOne(c => c.ProductItem)
               .HasForeignKey(p => p.ProductItemId)
               .OnDelete(DeleteBehavior.SetNull);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        builder.Property(p => p.VariantObjects)
            .HasConversion(
                v => JsonSerializer.Serialize(v, options),
                v => JsonSerializer.Deserialize<List<VariantObject>>(v, options) ?? new List<VariantObject>())
            .HasColumnType("jsonb");
    }
}