using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.DTOs.Common;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Exceptions;
using fashion_shop.Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace fashion_shop.Infrastructure.Services;

public class PersonalProfileService : IPersonalProfileService
{
    private readonly UserManager<User> _userManager;
    private readonly MinioSettings _minioSettings;
    private readonly IPasswordHasher<User> _passwordHasher;

    public PersonalProfileService(
        UserManager<User> userManager,
        IOptions<MinioSettings> options,
        IPasswordHasher<User> passwordHasher)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _minioSettings = options.Value ?? throw new ArgumentNullException(nameof(options.Value));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    }

    public async Task<GetMeResponse> GetMeAsync(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        return new GetMeResponse
        {
            Id = user.Id,
            Username = user.UserName ?? "",
            Email = user.Email ?? "",
            PhoneNumber = user.PhoneNumber ?? "",
            ImageUrl = !string.IsNullOrEmpty(user.ImageUrl) ? _minioSettings.GetUrlImage(user.ImageUrl, true, false) : null,
            Role = roles.ToList()
        };
    }

    public async Task UpdatePasswordAsync(User user, UpdateProfilePasswordRequest request)
    {
        var resultCheckPassword = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash ?? "", request.Password);

        if (resultCheckPassword == PasswordVerificationResult.Failed)
        {
            throw new UnAuthorizedException("Wrong Password");
        }

        user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);

        await _userManager.UpdateAsync(user);
    }
}