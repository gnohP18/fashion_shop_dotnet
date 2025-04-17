using fashion_shop.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fashion_shop.Infrastructure.Database.Configurations.Entities;

public class VariantConfiguration : ConfigurationEntity<Variant>
{
    public override void Configure(EntityTypeBuilder<Variant> builder)
    {
        base.Configure(builder);

        builder.ToTable("variants");

        builder.Property(p => p.Code)
               .HasMaxLength(10)
               .IsRequired();

        builder.Property(p => p.Value)
               .HasMaxLength(10)
               .IsRequired();

        builder.HasOne(p => p.ProductVariant)
                .WithMany(p => p.Variants)
                .HasForeignKey(p => p.ProductVariantId)
                .OnDelete(DeleteBehavior.Cascade);
    }
}