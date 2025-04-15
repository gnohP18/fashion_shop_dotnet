using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.DTOs.Requests.User;
using fashion_shop.Core.DTOs.Responses;
using fashion_shop.Core.DTOs.Responses.User;

namespace fashion_shop.Core.Interfaces.Services
{
    public interface IOrderService
    {
        Task<PaginationData<OrderDto>> GetHistoryOrderAsync(int userId, GetHistoryOrderRequest request);
    }
}