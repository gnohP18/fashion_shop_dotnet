using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.DTOs.Requests.Admin;

namespace fashion_shop.Core.Interfaces.Services;

public interface ISettingService
{
    Task UpdateSettingAsync<T>(string prefix, T request) where T : class;
    Task<T> GetSettingAsync<T>(string prefix) where T : new();
}