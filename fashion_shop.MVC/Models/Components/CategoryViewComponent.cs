using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace fashion_shop.MVC.Models.Components;

public class CategoryViewComponent : ViewComponent
{
    private readonly ICategoryService _categoryService;

    public CategoryViewComponent(ICategoryService categoryService)
    {
        _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var categoryGetParameter = new GetCategoryRequest();

        var categories = (await _categoryService.GetListAsync(categoryGetParameter)).Data.ToList();

        return View(categories);
    }
}