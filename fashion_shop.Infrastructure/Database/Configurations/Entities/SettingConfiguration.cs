using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fashion_shop.Infrastructure.Database.Configurations.Entities;

public class SettingConfiguration : ConfigurationEntity<Setting>
{
    public override void Configure(EntityTypeBuilder<Setting> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.Name)
           .HasMaxLength(255)
           .IsRequired();

        builder.Property(p => p.Value)
           .HasMaxLength(255)
           .IsRequired();

        builder.Property(p => p.DefaultValue)
           .HasMaxLength(255);
    }
}