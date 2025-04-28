using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.DTOs.Responses.Admin;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Interfaces.Services;
using fashion_shop.Infrastructure.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace fashion_shop.Infrastructure.Services;

public class UserManagementService : IUserManagementService
{
    private readonly IDatabase _redis;
    private readonly UserManager<User> _userManager;

    public UserManagementService(IConnectionMultiplexer connectionMultiplexer, UserManager<User> userManager)
    {
        _redis = connectionMultiplexer.GetDatabase() ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    public async Task<bool> CheckExistUserPhoneAsync(string phone)
    {
        var phoneHashed = Infrastructure.Common.Function.HashStringCRC32(phone);

        if (!await _redis.StringGetBitAsync(RedisConstant.USER_PHONE_LIST, phoneHashed))
        {
            return false;
        }

        return true;
    }

    public async Task<bool> CheckExistUserUsernameAsync(string username)
    {
        var usernameHashed = Infrastructure.Common.Function.HashStringCRC32(username);

        if (!await _redis.StringGetBitAsync(RedisConstant.USER_USERNAME_LIST, usernameHashed))
        {
            return false;
        }

        return true;
    }

    public async Task<UserForOrderResponse?> GetUserForOrderByPhoneAsync(string phone)
    {
        return await _userManager
            .Users
            .Where(u => u.PhoneNumber == phone)
            .Select(u => new UserForOrderResponse
            {
                Username = u.UserName ?? "",
                Phone = u.PhoneNumber ?? "",
                Email = u.Email ?? ""
            })
            .FirstOrDefaultAsync();
    }
}