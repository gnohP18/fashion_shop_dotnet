using fashion_shop.API.ExternalService.DTOs;
using fashion_shop.Core.DTOs;
using fashion_shop.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace fashion_shop.API.Controllers.Webhook;

[ApiController]
[Route("api/webhook")]
public class MediaFileController : APIController<MediaFileController>
{
    private readonly IMediaFileService _mediaFileService;

    public MediaFileController(
        ILogger<MediaFileController> logger,
        IMediaFileService mediaFileService) : base(logger)
    {
        _mediaFileService = mediaFileService ?? throw new ArgumentNullException(nameof(mediaFileService));
    }

    [HttpPost("minio/upload-image")]
    public async Task<IActionResult> UpdateMediaFile([FromBody] MinioResponse payload)
    {
        if (!ModelState.IsValid)
        {
            return ErrorResponse<string>("Validation Failed");
        }
        System.Console.WriteLine(payload.Key);
        var s3Key = Uri.UnescapeDataString(payload.Records[0].S3.Object.Key);
        System.Console.WriteLine(s3Key);

        await _mediaFileService.UpdateStatusMediaFileAsync(s3Key);

        return SuccessResponse<string>(string.Empty, "Success");
    }
}