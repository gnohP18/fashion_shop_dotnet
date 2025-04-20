using fashion_shop.Core.Common;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Interfaces.Repositories;

namespace fashion_shop.API.Seeders;

public static class SettingSeeder
{
    public static async Task Seed(IServiceScope scope)
    {
        var repositorySetting = scope.ServiceProvider.GetRequiredService<ISettingRepository>();

        var settings = new List<Setting> {
            new Setting
            {
                Name = $"{SettingPrefixConstants.BasicInfoPrefix}_{nameof(InfoSetting.ShopName).ToLower()}",
                Value = "Fashion shop",
            },
            new Setting
            {
                Name = $"{SettingPrefixConstants.BasicInfoPrefix}_{nameof(InfoSetting.ShopEmail).ToLower()}",
                Value = "fashionshop@gmail.com",
            },
            new Setting
            {
                Name = $"{SettingPrefixConstants.BasicInfoPrefix}_{nameof(InfoSetting.ShopAddress).ToLower()}",
                Value = "144 Tran Phu, Tam Ky, Quang Nam",
            },
            new Setting
            {
                Name = $"{SettingPrefixConstants.BasicInfoPrefix}_{nameof(InfoSetting.ShopPhone).ToLower()}",
                Value = "0912345678",
            }
        };

        await repositorySetting.AddManyAsync(settings);
        await repositorySetting.UnitOfWork.SaveChangesAsync();
    }
}