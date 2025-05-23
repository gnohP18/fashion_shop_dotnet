using fashion_shop.Core.Common;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.Core.Interfaces.Services;
using fashion_shop.Infrastructure.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace fashion_shop.Infrastructure.Services;

public class SettingService : ISettingService
{
    private readonly ISettingRepository _settingRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IDatabase _redis;
    private readonly ILogger _logger;
    private readonly UserManager<User> _userManager;
    private readonly ICurrenUserContext _currenUserContext;

    public SettingService(
        ISettingRepository settingRepository,
        ICategoryRepository categoryRepository,
        IConnectionMultiplexer connectionMultiplexer,
        ILogger<SettingService> logger,
        UserManager<User> userManager,
        ICurrenUserContext currenUserContext)
    {
        _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        _redis = connectionMultiplexer.GetDatabase() ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _currenUserContext = currenUserContext ?? throw new ArgumentNullException(nameof(currenUserContext));
    }

    public async Task<T> GetSettingAsync<T>(string prefix) where T : new()
    {
        var props = typeof(T).GetProperties();

        var userId = string.Empty;

        if (!IsSystemSetting(prefix))
        {
            userId = string.IsNullOrEmpty(_currenUserContext.UserId) ? string.Empty : $"{_currenUserContext.UserId}_";
        }

        var keys = props.Select(p => $"{prefix}_{userId}{p.Name.ToLower()}").ToList();

        var settings = await _settingRepository
            .Queryable
            .WithoutDeleted()
            .Where(s => keys.Contains(s.Name))
            .ToListAsync();

        var result = new T();

        foreach (var prop in props)
        {
            var key = $"{prefix}_{userId}{prop.Name.ToLower()}";

            var setting = settings.FirstOrDefault(s => s.Name == key);

            if (setting != null && prop.CanWrite)
            {
                var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                object value;

                if (targetType.IsEnum)
                {
                    value = Enum.Parse(targetType, setting.Value);
                }
                else
                {
                    value = Convert.ChangeType(setting.Value, targetType);
                }

                prop.SetValue(result, value);
            }
        }

        return result;
    }

    public async Task UpdateSettingAsync<T>(string prefix, T request) where T : class
    {
        var settings = MapToSettings(prefix, request);

        var names = settings.Select(s => s.Name).ToList();

        var existingSettings = await _settingRepository
            .Queryable
            .WithoutDeleted()
            .Where(s => names.Contains(s.Name))
            .ToListAsync();

        _settingRepository.DeleteMany(existingSettings);
        await _settingRepository.AddManyAsync(settings);
        await _settingRepository.UnitOfWork.SaveChangesAsync();
    }

    private List<Setting> MapToSettings<T>(string prefix, T request) where T : class
    {
        var userId = string.Empty;

        if (!IsSystemSetting(prefix))
        {
            userId = string.IsNullOrEmpty(_currenUserContext.UserId) ? string.Empty : $"{_currenUserContext.UserId}_";
        }

        return typeof(T)
            .GetProperties()
            .Select(p => new Setting
            {
                Name = $"{prefix}_{userId}{p.Name.ToLower()}",
                Value = p.GetValue(request)?.ToString() ?? string.Empty
            }).ToList();
    }

    public async Task SyncRedisDataAsync()
    {
        await SyncCategoryAsync();
        await SyncUserAsync();
    }

    private async Task SyncCategoryAsync()
    {
        // Clear key
        await _redis.KeyDeleteAsync(RedisConstant.CATEGORY_LIST);

        // Sync Category slug
        var slugOfcategories = await _categoryRepository.Queryable.Select(c => c.Slug).ToListAsync();

        foreach (var slug in slugOfcategories)
        {
            var hashedSlug = Infrastructure.Common.Function.HashStringCRC32(slug);

            _logger.LogInformation("Hashed {0} ===> {1}", slug, hashedSlug);
            await _redis.StringSetBitAsync(RedisConstant.CATEGORY_LIST, hashedSlug, true);
        }
    }

    private async Task SyncUserAsync()
    {
        // Clear key
        await _redis.KeyDeleteAsync(RedisConstant.USER_USERNAME_LIST);
        await _redis.KeyDeleteAsync(RedisConstant.USER_PHONE_LIST);

        // get all user
        var users = await _userManager
            .Users.Select(u => new
            {
                Username = u.UserName,
                Phone = u.PhoneNumber,
            }).ToListAsync();

        foreach (var user in users)
        {
            if (!string.IsNullOrEmpty(user?.Username))
            {
                var hashedUsername = Infrastructure.Common.Function.HashStringCRC32(user?.Username ?? "");

                _logger.LogInformation("Hashed {0} ===> {1}", user?.Username, hashedUsername);
                await _redis.StringSetBitAsync(RedisConstant.USER_USERNAME_LIST, hashedUsername, true);
            }

            if (!string.IsNullOrEmpty(user?.Phone))
            {
                var hashedPhone = Infrastructure.Common.Function.HashStringCRC32(user?.Phone ?? "");

                _logger.LogInformation("Hashed {0} ===> {1}", user?.Phone, hashedPhone);
                await _redis.StringSetBitAsync(RedisConstant.USER_PHONE_LIST, hashedPhone, true);
            }
        }
    }

    private bool IsSystemSetting(string prefix)
    {
        var systemSettingPrefix = new List<string> {
            SettingPrefixConstants.BasicInfoPrefix,
            SettingPrefixConstants.PaymentInfoPrefix
        };

        return systemSettingPrefix.Contains(prefix);
    }
}