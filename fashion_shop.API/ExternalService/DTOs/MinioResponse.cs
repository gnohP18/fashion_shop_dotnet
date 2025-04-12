using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.API.ExternalService.DTOs;

public class MinioResponse
{
    public string EventName { get; set; } = default!;
    public string Key { get; set; } = default!;
    public List<MinioRecord> Records { get; set; } = new List<MinioRecord>();
}

public class MinioRecord
{
    public MinioRecordS3 S3 { get; set; } = default!;
}

public class MinioRecordS3
{
    public MinioS3Object Object { get; set; } = default!;
}

public class MinioS3Object
{
    public string Key { get; set; } = default!;
}