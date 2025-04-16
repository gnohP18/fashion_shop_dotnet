using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.DTOs.Requests.User;
using fashion_shop.Core.DTOs.Responses;
using fashion_shop.Core.DTOs.Responses.User;
using fashion_shop.Core.Exceptions;
using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.Core.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace fashion_shop.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;

        public OrderService(
            IOrderRepository orderRepository,
            IOrderDetailRepository orderDetailRepository)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
        }

        public async Task<PaginationData<OrderDto>> GetHistoryOrderAsync(int userId, GetHistoryOrderRequest request)
        {
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

        public async Task<OrderDto?> GetOrderAsync(int orderId)
        {
            return await _orderRepository.Queryable
                .AsNoTracking()
                .Select(o => new OrderDto()
                {
                    Id = o.Id,
                    TotalAmount = o.TotalAmount,
                    Note = o.Note,
                    CreatedAt = o.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                }).FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<OrderDetailResponse> GetOrderDetailAsync(OrderDto order)
        {
            var orderDetails = await _orderDetailRepository
                .Queryable
                .AsNoTracking()
                .Include(o => o.Product)
                .Where(o => o.OrderId == order.Id)
                .Select(o => new OrderDetailDto()
                {
                    OrderId = o.OrderId,
                    ProductId = o.ProductId,
                    ProductName = o.ProductName,
                    Quantity = o.Quantity,
                    Price = o.Price,
                    ImageUrl = o.Product.ImageUrl ?? "https://picsum.photos/64"
                }).ToListAsync();

            return new OrderDetailResponse()
            {
                OrderId = order.Id,
                TotalAmount = order.TotalAmount,
                Note = order.Note,
                CreatedAt = order.CreatedAt,
                OrderDetail = orderDetails
            };
        }
    }
}