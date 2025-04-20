using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.DTOs.Responses;
using fashion_shop.Core.DTOs.Responses.Admin;

namespace fashion_shop.Core.Interfaces.Services;

public interface IProductService
{
    Task<ProductDto?> GetDetailAsync(int id);
    Task<ProductDto?> GetDetailBySlugAsync(string slug);
    Task<PaginationData<BasicProductDto>> GetListAsync(GetProductRequest productId);
    Task CreateAsync(CreateProductRequest request);
    Task UpdateBasicInfoAsync(int id, UpdateProductRequest request);

    /// <summary>
    /// IsVariant
    /// prev: True -> next: False -> Disable <br/>
    /// prev: False -> next: True -> Create New <br/>
    /// prev: True -> next: True -> Update ProductItem
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request">UpdateProductVariantRequest</param>
    /// <returns></returns>
    Task UpdateProductVariantAsync(int id, UpdateProductVariantRequest request);
    Task DeleteAsync(int id);
}