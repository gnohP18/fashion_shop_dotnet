using fashion_shop.Core.DTOs.Common;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.DTOs.Responses;
using fashion_shop.Core.DTOs.Responses.Admin;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Interfaces.Services;
using fashion_shop.Infrastructure.Common;
using fashion_shop.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace fashion_shop.Infrastructure.Services;

public class UserManagementService : IUserManagementService
{
    private readonly IDatabase _redis;
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _dbContext;
    private readonly MinioSettings _minioSettings;
    private readonly ICurrenUserContext _currenUserContext;

    public UserManagementService(
        IConnectionMultiplexer connectionMultiplexer,
        UserManager<User> userManager,
        ApplicationDbContext dbContext,
        IOptions<MinioSettings> options,
        ICurrenUserContext currenUserContext)
    {
        _redis = connectionMultiplexer.GetDatabase() ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _minioSettings = options.Value ?? throw new ArgumentNullException(nameof(options));
        _currenUserContext = currenUserContext ?? throw new ArgumentNullException(nameof(currenUserContext));
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

    public async Task<PaginationData<BasicUserResponse>> GetListUserAsync(GetListUserRequest request)
    {
        var query = from user in _dbContext.Users.AsQueryable().AsNoTracking()
                    join userRole in _dbContext.UserRoles on user.Id equals userRole.UserId
                    join role in _dbContext.Roles on userRole.RoleId equals role.Id
                    select new
                    {
                        user,
                        roleName = role.Name
                    };

        if (!string.IsNullOrEmpty(_currenUserContext.UserId))
        {
            query = query.Where(u => u.user.Id != Int32.Parse(_currenUserContext.UserId));
        }

        if (!string.IsNullOrEmpty(request.KeySearch))
        {
            query = query.Where(
                x => EF.Functions.ILike(x.user.UserName ?? "", $"%{request.KeySearch}%") ||
                    EF.Functions.ILike(x.user.PhoneNumber ?? "", request.KeySearch));
        }

        var total = await query.CountAsync();

        query = query.OrderBy(x => x.user.Id);

        var data = await query
            .Select(x => new BasicUserResponse
            {
                Id = x.user.Id,
                Username = x.user.UserName ?? "",
                PhoneNumber = x.user.PhoneNumber ?? "",
                RoleName = x.roleName ?? "N/A",
                ImageUrl = !string.IsNullOrWhiteSpace(x.user.ImageUrl) ?
                    _minioSettings.GetUrlImage(x.user.ImageUrl, true, false) : null
            })
            .Skip((request.Page - 1) * request.Offset)
            .Take(request.Offset)
            .ToListAsync();

        return new PaginationData<BasicUserResponse>(data, request.Offset, request.Page, total);
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