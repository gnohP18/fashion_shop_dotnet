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

        var tshirtCategory = new Category { Name = "T-Shirt", Slug = "t-shirt" };
        var hatCategory = new Category { Name = "Hat", Slug = "hat" };

        await serviceCategory.AddAsync(tshirtCategory);
        await serviceCategory.AddAsync(hatCategory);

        // Seed Products
        var tshirtProducts = new List<Product>
        {
            new Product { Name = "Classic Tee", Slug = "classic-tee", Price = "19.99", ImageUrl = "classic.jpg", Description = "A classic t-shirt", Category = tshirtCategory },
            new Product { Name = "Graphic Tee", Slug = "graphic-tee", Price = "24.99", ImageUrl = "graphic.jpg", Description = "A graphic t-shirt", Category = tshirtCategory }
        };

        var hatProducts = new List<Product>
        {
            new Product { Name = "Baseball Cap", Slug = "baseball-cap", Price = "14.99", ImageUrl = "cap.jpg", Description = "A cool cap", Category = hatCategory },
            new Product { Name = "Beanie", Slug = "beanie", Price = "18.99", ImageUrl = "beanie.jpg", Description = "A warm beanie", Category = hatCategory }
        };

        foreach (var product in tshirtProducts.Concat(hatProducts))
        {
            await serviceProduct.AddAsync(product);
        }
    }
}