using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.DTOs.Common;

public class CreatePresignedUrlRequest
{
    public string ContentType { get; set; } = default!;
    public string FileName { get; set; } = default!;
    public int FileSize { get; set; }
}