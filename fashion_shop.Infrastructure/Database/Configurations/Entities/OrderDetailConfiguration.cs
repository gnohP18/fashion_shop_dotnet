using fashion_shop.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fashion_shop.Infrastructure.Database.Configurations.Entities;

public class OrderDetailConfiguration : ConfigurationEntity<OrderDetail>
{

    public override void Configure(EntityTypeBuilder<OrderDetail> builder)
    {
        base.Configure(builder);

        builder.ToTable("order_details");

        builder.Property(p => p.ProductName)
               .HasMaxLength(255)
               .IsRequired(true);

        builder.Property(p => p.Quantity)
               .IsRequired(true);

        builder.Property(p => p.Price)
               .IsRequired(true);

        builder.HasIndex(p => new { p.OrderId, p.ProductId }).IsUnique();

        builder.HasOne(p => p.Product)
               .WithMany(c => c.OrderDetails)
               .HasForeignKey(p => p.ProductId);

        builder.HasOne(p => p.Order)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(p => p.OrderId);
    }
}
