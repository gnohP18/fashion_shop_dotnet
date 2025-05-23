using fashion_shop.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fashion_shop.Infrastructure.Database.Configurations.Entities;

public class OrderConfiguration : ConfigurationEntity<Order>
{

    public override void Configure(EntityTypeBuilder<Order> builder)
    {
        base.Configure(builder);

        builder.ToTable("orders");

        builder.Property(p => p.TotalAmount)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(p => p.Note)
               .HasMaxLength(255)
               .IsRequired(false);

        builder.HasOne(p => p.User)
                .WithMany(p => p.Orders)
                .HasForeignKey(p => p.UserId);

        builder.HasMany(o => o.PaymentTransactions)
               .WithOne(pt => pt.Order)
               .HasForeignKey(pt => pt.OrderId);
    }
}
