using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.Entities;

public class MediaFile : BaseEntity
{
    public string FileName { get; set; } = default!;
    public string FileExtension { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public string ObjectType { get; set; } = default!;
    public int ObjectId { get; set; }
    public string? S3Key { get; set; }
    public bool IsUpload { get; set; } = false;
}