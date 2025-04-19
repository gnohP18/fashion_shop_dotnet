using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fashion_shop.Infrastructure.Database.Configurations.Entities;

public class PaymentTransactionConfiguration : ConfigurationEntity<PaymentTransaction>
{
    public override void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        base.Configure(builder);

        builder.ToTable("payment_transactions");

        builder.Property(p => p.TotalAmount)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(p => p.Currency)
               .HasMaxLength(10)
               .HasDefaultValue("VND")
               .IsRequired();

        builder.Property(p => p.Status)
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(p => p.Method)
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(p => p.ProviderTransactionId)
               .HasMaxLength(255);

        builder.Property(p => p.Signature)
               .HasMaxLength(512);

        builder.Property(p => p.RawResponse)
               .HasColumnType("text");

        builder.Property(p => p.FailReason)
               .HasMaxLength(1000);

        builder.Property(p => p.IpnVerified)
               .HasDefaultValue(false);

        builder.Property(p => p.PaymentTime)
               .IsRequired(false);

        builder.HasOne(p => p.Order)
               .WithMany(o => o.PaymentTransactions)
               .HasForeignKey(p => p.OrderId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.ProviderTransactionId).IsUnique(false);
    }
}