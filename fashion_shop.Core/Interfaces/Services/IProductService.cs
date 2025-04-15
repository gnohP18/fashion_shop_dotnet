using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.DTOs.Responses;
using fashion_shop.Core.DTOs.Responses.Admin;

namespace fashion_shop.Core.Interfaces.Services;

public interface IProductService
{
    Task<ProductDto?> GetDetailAsync(int id);
    Task<ProductDto?> GetDetailBySlugAsync(string slug);
    Task<PaginationData<ProductDto>> GetListAsync(GetProductRequest productId);
    Task<CreateProductResponse> CreateAsync(CreateProductRequest request);
    Task DeleteAsync(int id);
}