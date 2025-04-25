using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.DTOs.Requests.User;
using fashion_shop.Core.DTOs.Responses;
using fashion_shop.Core.DTOs.Responses.Admin;
using fashion_shop.Core.DTOs.Responses.User;

namespace fashion_shop.Core.Interfaces.Services
{
    public interface IOrderService
    {
        Task<PaginationData<BasicOrderResponse>> GetListOrder(GetListOrderRequest request);
        Task<OrderDto?> GetOrderAsync(int orderId);
        Task<PaginationData<OrderDto>> GetHistoryOrderAsync(int userId, GetHistoryOrderRequest request);
        Task<OrderDetailResponse> GetOrderDetailAsync(OrderDto order);
    }
}