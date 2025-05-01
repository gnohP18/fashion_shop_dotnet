using fashion_shop.API.ExternalService.DTOs;
using fashion_shop.Core.DTOs;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace fashion_shop.API.Controllers.Webhook;

[ApiController]
[Route("api/webhook")]
[Tags("Media File")]
public class MediaFileController : APIController<MediaFileController>
{
    private readonly IMediaFileService _mediaFileService;

    public MediaFileController(
        ILogger<MediaFileController> logger,
        IHttpContextAccessor httpContextAccessor,
        UserManager<User> userManager,
        IMediaFileService mediaFileService) : base(logger, httpContextAccessor, userManager)
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

        var s3Key = Uri.UnescapeDataString(payload.Records[0].S3.Object.Key);

        await _mediaFileService.UpdateStatusMediaFileAsync(s3Key);

        return OkResponse<string>(string.Empty, "Success");
    }
}