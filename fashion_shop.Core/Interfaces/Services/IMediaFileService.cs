using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.DTOs.Common;

namespace fashion_shop.Core.Interfaces.Services;

public interface IMediaFileService
{
    Task<string> UploadFileAsync(CreateMediaFileRequest fileStream);
    Task<string> CreatePresignedUrlAsync(CreatePresignedUrlRequest request, string objectType, int objectId);
    Task UpdateStatusMediaFileAsync(string s3Key);
}