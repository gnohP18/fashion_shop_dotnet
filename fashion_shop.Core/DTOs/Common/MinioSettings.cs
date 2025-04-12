using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.Core.DTOs.Common;

public class MinioSettings
{
    public string BucketName { get; set; } = String.Empty;
    public string Endpoint { get; set; } = String.Empty;
    public string AccessKey { get; set; } = String.Empty;
    public string SecretKey { get; set; } = String.Empty;
    public int ExpiredHoursPresignUrl { get; set; }
}