using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.Common;
using fashion_shop.Core.DTOs.Responses.User;
using fashion_shop.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace fashion_shop.MVC.Models.Components;

public class FooterViewComponent : ViewComponent
{
    private readonly ISettingService _settingService;

    public FooterViewComponent(ISettingService settingService)
    {
        _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        ViewBag.BasicInfo = await _settingService.GetSettingAsync<BasicInfoSettingResponse>(SettingPrefixConstants.BasicInfoPrefix);

        return View();
    }
}