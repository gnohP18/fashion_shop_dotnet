using fashion_shop.Core.DTOs.Responses.User;

namespace fashion_shop.Core.Interfaces.Services;

public interface ICartService
{
    Task<Dictionary<CartItemDto, int>> GetListAsync(Dictionary<int, int> cartItems);
    Task<bool> CheckProductExistByProductItemIdAsync(int productItemId);
    Task<bool> CheckoutCartAsync(int userId, Dictionary<int, int> cartItems);
}