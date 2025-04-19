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

        var category1 = await serviceCategory.CreateAsync(tshirtCategory);
        var category2 = await serviceCategory.CreateAsync(hatCategory);

        // product2
        var productVariants1 = new List<CreateProductVariantRequest> {
            new CreateProductVariantRequest { Name = "Color", Priority = 1 },
            new CreateProductVariantRequest { Name = "Size", Priority = 2 },
            new CreateProductVariantRequest { Name = "Texture", Priority = 3 },
        };

        var variants1 = new List<CreateVariantRequest>(){
            // product2 -> product_variant1
            new CreateVariantRequest { Priority = 1, Value = "Blue"},
            new CreateVariantRequest { Priority = 1, Value = "Black"},
            new CreateVariantRequest { Priority = 1, Value = "White"},
            // product2 -> product_variant2
            new CreateVariantRequest { Priority = 2, Value = "S"},
            new CreateVariantRequest { Priority = 2, Value = "M"},
            new CreateVariantRequest { Priority = 2, Value = "L"},
            // product2 -> product_variant3
            new CreateVariantRequest { Priority = 3, Value = "Cotton"},
            new CreateVariantRequest { Priority = 3, Value = "Wool"},
            new CreateVariantRequest { Priority = 3, Value = "Silk"}
        };

        // Seed Products
        var tshirtProducts = new List<CreateProductRequest>
        {
            new CreateProductRequest {
                Name = "Classic Tee",
                Slug = "classic-tee",
                Price = 400000,
                Description = "Áo thun đủ màu mua ngày đi kẻo ế",
                ImageUrl = "",
                CategoryId = category1.Id,
                IsVariant = false,
            },
            new CreateProductRequest {
                Name = "Graphic Tee",
                Slug = "graphic-tee",
                Price = 203000,
                Description = "Áo thun đủ màu mua ngày đi kẻo ế",
                ImageUrl = "",
                CategoryId = category1.Id,
                IsVariant = true,
                ProductVariants = productVariants1,
                Variants = variants1
            }
        };

        var hatProducts = new List<CreateProductRequest>
        {
            new CreateProductRequest {
                Name = "Baseball Cap",
                Slug = "baseball-cap",
                Price = 245000,
                ImageUrl = "",
                Description = "A cool cap",
                CategoryId = category2.Id,
                IsVariant = false,
            },
            new CreateProductRequest {
                Name = "Beanie",
                Slug = "beanie",
                Price = 210000,
                ImageUrl = "",
                Description = "A warm beanie",
                CategoryId = category2.Id,
                IsVariant = true,
                ProductVariants = productVariants1,
                Variants = variants1
            }
        };

        foreach (var product in tshirtProducts.Concat(hatProducts))
        {
            await serviceProduct.CreateAsync(product);
        }
    }
}