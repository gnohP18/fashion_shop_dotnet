using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.DTOs.Common;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Exceptions;
using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.Core.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.DataModel.Select;
using Minio.Exceptions;

namespace fashion_shop.Infrastructure.Services
{
    public class MediaFileService : IMediaFileService
    {
        private readonly IMinioClient _minioClient;
        private readonly MinioSettings _minio;
        private readonly IMediaFileRepository _mediaFileRepository;
        private readonly IProductRepository _productRepository;

        public MediaFileService(
            IOptions<MinioSettings> options,
            IMinioClient minioClient,
            IMediaFileRepository mediaFileRepository,
            IProductRepository productRepository)
        {
            _minio = options.Value ?? throw new ArgumentNullException(nameof(options));
            _minioClient = minioClient ?? throw new ArgumentNullException(nameof(minioClient));
            _mediaFileRepository = mediaFileRepository ?? throw new ArgumentNullException(nameof(mediaFileRepository));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        public async Task<string> CreatePresignedUrlAsync(CreatePresignedUrlRequest request, string objectType, int objectId)
        {
            var fileExtension = request.ContentType.Split("/")[1];

            // create s3 Key
            var s3Key = $"{objectType}/{Guid.NewGuid()}.{fileExtension}";

            // check exist record
            await CheckExistMediaFileRecord(objectType, objectId, s3Key);

            // Check exist bucket
            var bucketExists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_minio.BucketName));
            if (!bucketExists)
            {
                await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_minio.BucketName));
            }

            // create mediaFile
            var mediaFile = new MediaFile()
            {
                FileName = request.FileName,
                FileExtension = fileExtension,
                ContentType = request.ContentType,
                ObjectType = objectType,
                ObjectId = objectId,
                S3Key = s3Key,
                IsUpload = false
            };

            await _mediaFileRepository.AddAsync(mediaFile);
            await _mediaFileRepository.UnitOfWork.SaveChangesAsync();

            // Create PresignUrl
            var presignedUrl = await _minioClient.PresignedPutObjectAsync(new PresignedPutObjectArgs()
                .WithBucket(_minio.BucketName)
                .WithObject(s3Key)
                .WithExpiry(_minio.ExpiredHoursPresignUrl * 3600));

            return presignedUrl;
        }

        public async Task<string> UploadFileAsync(CreateMediaFileRequest request)
        {
            var s3Key = $"{request.ObjectType}/{Guid.NewGuid()}.{request.FileExtension}";

            var bucketExists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_minio.BucketName));

            if (!bucketExists)
            {
                await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_minio.BucketName));
            }

            await CheckExistMediaFileRecord(request.ObjectType, request.ObjectId, s3Key);

            var mediaFile = new MediaFile()
            {
                FileName = request.FileName,
                FileExtension = request.FileExtension,
                ContentType = request.ContentType,
                ObjectType = request.ObjectType,
                ObjectId = request.ObjectId,
                S3Key = s3Key,
                IsUpload = true
            };

            await _mediaFileRepository.AddAsync(mediaFile);


            var resp = await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(_minio.BucketName)
                .WithObject(s3Key)
                .WithStreamData(request.FileStream)
                .WithObjectSize(request.FileStream.Length)
                .WithContentType(request.ContentType)
            );

            await UpdateImageUrl(objectType: request.ObjectType, objectId: request.ObjectId, s3Key);

            await _mediaFileRepository.UnitOfWork.SaveChangesAsync();

            return $"http://{_minio.Endpoint}/{_minio.BucketName}/{s3Key}";
        }

        public async Task UpdateStatusMediaFileAsync(string s3Key)
        {
            var mediaFile = await _mediaFileRepository.Queryable.FirstOrDefaultAsync(s => s.S3Key == s3Key);

            if (mediaFile == null)
            {
                throw new NotFoundException($"Not found mediaFile s3Key={s3Key}");
            }

            mediaFile.IsUpload = true;
            _mediaFileRepository.Update(mediaFile);

            if (mediaFile.S3Key is not null)
            {
                await UpdateImageUrl(mediaFile.ObjectType, mediaFile.ObjectId, mediaFile.S3Key);
            }

            await _mediaFileRepository.UnitOfWork.SaveChangesAsync();
        }

        private async Task CheckExistMediaFileRecord(string objectType, int objectId, string s3Key)
        {
            var existedRecord = await _mediaFileRepository.Queryable
                .FirstOrDefaultAsync(_ => _.ObjectId == objectId && _.ObjectType == objectType);

            if (existedRecord is not null)
            {
                _mediaFileRepository.Delete(existedRecord);
                await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                    .WithBucket(_minio.BucketName)
                    .WithObject(s3Key));
            }
        }

        private async Task UpdateImageUrl(string objectType, int objectId, string imageUrl)
        {
            switch (objectType)
            {
                case "product":
                    var product = await _productRepository.Queryable.FirstOrDefaultAsync(_ => _.Id == objectId);

                    if (product is null)
                    {
                        throw new NotFoundException($"Not Found Product Id={objectId}");
                    }

                    product.ImageUrl = imageUrl;

                    break;
                default:
                    throw new NotSupportedException($"ObjectType '{objectType}' is not supported.");
            }

        }
    }
}