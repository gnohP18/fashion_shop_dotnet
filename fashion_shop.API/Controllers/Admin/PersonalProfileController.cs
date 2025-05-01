using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.Internal.Mappers;
using fashion_shop.API.Attributes;
using fashion_shop.Core.Common;
using fashion_shop.Core.DTOs.Common;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Exceptions;
using fashion_shop.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace fashion_shop.API.Controllers.Admin;

[Authenticate]
[ApiController]
[Tags("Personal Profile")]
[Route("api/me")]
public class PersonalProfileController : APIController<PersonalProfileController>
{
    private readonly IPersonalProfileService _personalProfileService;
    private readonly IMediaFileService _mediaFileService;
    private readonly IAdminAuthService _adminAuthService;

    public PersonalProfileController(
        ILogger<PersonalProfileController> logger,
        IPersonalProfileService personalProfileService,
        IHttpContextAccessor httpContextAccessor,
        UserManager<User> userManager,
        IMediaFileService mediaFileService,
        IAdminAuthService adminAuthService) : base(logger, httpContextAccessor, userManager)
    {
        _personalProfileService = personalProfileService ?? throw new ArgumentNullException(nameof(personalProfileService));
        _mediaFileService = mediaFileService ?? throw new ArgumentNullException(nameof(mediaFileService));
        _adminAuthService = adminAuthService ?? throw new ArgumentNullException(nameof(adminAuthService));
    }

    [HttpGet("")]
    public async Task<IActionResult> GetMe()
    {
        var user = await GetUser();

        var data = await _personalProfileService.GetMeAsync(user);

        return OkResponse<GetMeResponse>(data, "Get data successfully");
    }

    [HttpPost("presigned-upload")]
    public async Task<IActionResult> CreatePresignUploadAvatar([FromBody] CreatePresignedUrlRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ErrorResponse<string>("Validation Failed");
        }

        var user = await GetUser();

        var presignUrl = await _mediaFileService
            .CreatePresignedUrlAsync(request, nameof(User).ToLower(), user.Id);

        return OkResponse<string>(presignUrl, "Created successfully");
    }

    [Authenticate(RoleContants.Admin, RoleContants.Manager)]
    [HttpPut("change-password")]
    public async Task UpdatePassword([FromBody] UpdateProfilePasswordRequest request)
    {
        var user = await GetUser();

        var jti = GetJti();

        await _personalProfileService.UpdatePasswordAsync(user, request);
        await _adminAuthService.LogoutAsync(user.Id.ToString(), jti);
    }
}