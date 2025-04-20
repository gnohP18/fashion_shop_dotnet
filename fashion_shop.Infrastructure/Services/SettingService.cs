using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.Common;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.Core.Interfaces.Services;
using fashion_shop.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace fashion_shop.Infrastructure.Services;

public class SettingService : ISettingService
{
    private readonly ISettingRepository _settingRepository;

    public SettingService(ISettingRepository settingRepository)
    {
        _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
    }

    public async Task<T> GetSettingAsync<T>(string prefix) where T : new()
    {
        var props = typeof(T).GetProperties();

        var keys = props.Select(p => $"{prefix}_{p.Name.ToLower()}").ToList();

        var settings = await _settingRepository
            .Queryable
            .WithoutDeleted()
            .Where(s => keys.Contains(s.Name))
            .ToListAsync();

        var result = new T();

        foreach (var prop in props)
        {
            var key = $"{prefix}_{prop.Name.ToLower()}";
            var setting = settings.FirstOrDefault(s => s.Name == key);

            if (setting != null && prop.CanWrite)
            {
                var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                var value = Convert.ChangeType(setting.Value, targetType);
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
        return typeof(T)
            .GetProperties()
            .Select(p => new Setting
            {
                Name = $"{prefix}_{p.Name.ToLower()}",
                Value = p.GetValue(request)?.ToString() ?? string.Empty
            }).ToList();
    }
}