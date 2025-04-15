using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.DTOs.Requests.User;
using fashion_shop.Core.DTOs.Responses;
using fashion_shop.Core.DTOs.Responses.User;
using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.Core.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace fashion_shop.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<PaginationData<OrderDto>> GetHistoryOrderAsync(int userId, GetHistoryOrderRequest request)
        {
            System.Console.WriteLine(userId);
            var query = _orderRepository.Queryable
                .AsNoTracking()
                .Where(x => x.UserId == userId);

            var total = query.Count();

            query = query.Include(x => x.OrderDetails);

            var data = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((request.Page - 1) * request.Offset)
                .Take(request.Offset)
                .Select(o => new OrderDto()
                {
                    Id = o.Id,
                    TotalAmount = o.TotalAmount,
                    Note = o.Note,
                    CreatedAt = o.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                })
                .ToListAsync();

            return new PaginationData<OrderDto>(data, request.Offset, request.Page, total);
        }
    }
}