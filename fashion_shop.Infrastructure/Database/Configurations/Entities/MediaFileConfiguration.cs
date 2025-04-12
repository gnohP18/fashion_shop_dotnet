using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fashion_shop.Infrastructure.Database.Configurations.Entities
{
    public class MediaFileConfiguration : ConfigurationEntity<MediaFile>
    {
        public override void Configure(EntityTypeBuilder<MediaFile> builder)
        {
            base.Configure(builder);

            builder.ToTable("media_files");

            builder.Property(p => p.FileName)
                   .HasMaxLength(255)
                   .IsRequired();

            builder.Property(p => p.FileExtension)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(p => p.ContentType)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(p => p.ObjectType)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(p => p.ObjectId)
                   .IsRequired();

            builder.Property(p => p.S3Key)
                   .HasMaxLength(255);

            builder.Property(p => p.IsUpload)
                   .HasDefaultValue(false);
        }
    }
}