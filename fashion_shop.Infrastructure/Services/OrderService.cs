using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.DTOs.Common;
using fashion_shop.Core.DTOs.Requests.User;
using fashion_shop.Core.DTOs.Responses;
using fashion_shop.Core.DTOs.Responses.User;
using fashion_shop.Core.Exceptions;
using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.Core.Interfaces.Services;
using fashion_shop.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace fashion_shop.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly MinioSettings _minioSettings;

        public OrderService(
            IOrderRepository orderRepository,
            IOptions<MinioSettings> options,
            IOrderDetailRepository orderDetailRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _orderDetailRepository = orderDetailRepository ?? throw new ArgumentNullException(nameof(orderDetailRepository));
            _minioSettings = options.Value ?? throw new ArgumentNullException(nameof(options));
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
                .Include(o => o.ProductItem)
                .Where(o => o.OrderId == order.Id)
                .Select(o => new OrderDetailDto()
                {
                    OrderId = o.OrderId,
                    ProductId = o.ProductId,
                    ProductItemCode = o.ProductItem.Code,
                    ProductItemId = o.ProductItemId,
                    ProductSlug = o.Product.Slug,
                    ProductName = o.ProductName,
                    Quantity = o.Quantity,
                    Price = o.Price,
                    ImageUrl = !string.IsNullOrWhiteSpace(o.ProductItem.ImageUrl) ?
                        $"{_minioSettings.Endpoint}/{_minioSettings.BucketName}/{o.ProductItem.ImageUrl}" : ProductConstant.DefaultImage200
                }).ToListAsync();

            orderDetails.ForEach(orderDetail =>
            {
                orderDetail.Variants = orderDetail.ProductItemCode == "_" ? new List<string>() : orderDetail.ProductItemCode.Split("_").ToList();
            });

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