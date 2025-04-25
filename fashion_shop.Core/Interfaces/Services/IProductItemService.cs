using fashion_shop.Core.DTOs.Responses.Admin;

namespace fashion_shop.Core.Interfaces.Services
{
    public interface IProductItemService
    {
        Task<ProductItemDto?> GetDetailAsync(int id);
        Task<List<ProductItemDto>> GetListProductItemByProductId(int productId);
    }
}