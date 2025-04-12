using AutoMapper;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Interfaces.Services;

namespace fashion_shop.API.Seeders;

public static class ProductCategorySeeder
{
    public static async Task Seed(IServiceScope scope)
    {
        var serviceCategory = scope.ServiceProvider.GetRequiredService<ICategoryService>();
        var serviceProduct = scope.ServiceProvider.GetRequiredService<IProductService>();

        var tshirtCategory = new CreateCategoryRequest { Name = "T-Shirt", Slug = "t-shirt" };
        var hatCategory = new CreateCategoryRequest { Name = "Hat", Slug = "hat" };

        var res1 = await serviceCategory.CreateAsync(tshirtCategory);
        var res2 = await serviceCategory.CreateAsync(hatCategory);

        // Seed Products
        var tshirtProducts = new List<CreateProductRequest>
        {
            new CreateProductRequest { Name = "Classic Tee", Slug = "classic-tee", Price = 400000, ImageUrl = "classic.jpg", Description = "A classic t-shirt", CategoryId = res1.Id },
            new CreateProductRequest { Name = "Graphic Tee", Slug = "graphic-tee", Price = 203000, ImageUrl = "graphic.jpg", Description = "A graphic t-shirt", CategoryId = res1.Id }
        };

        var hatProducts = new List<CreateProductRequest>
        {
            new CreateProductRequest { Name = "Baseball Cap", Slug = "baseball-cap", Price = 245000, ImageUrl = "cap.jpg", Description = "A cool cap", CategoryId = res2.Id},
            new CreateProductRequest { Name = "Beanie", Slug = "beanie", Price = 210000, ImageUrl = "beanie.jpg", Description = "A warm beanie", CategoryId = res2.Id }
        };

        foreach (var product in tshirtProducts.Concat(hatProducts))
        {
            await serviceProduct.CreateAsync(product);
        }
    }
}