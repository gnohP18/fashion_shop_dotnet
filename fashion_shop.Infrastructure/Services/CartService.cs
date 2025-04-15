using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.DTOs.Common;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace fashion_shop.Infrastructure.Services;

public class CartService : ICartService
{
    private readonly IProductRepository _productRepository;
    private readonly UserManager<User> _userManager;
    private readonly MinioSettings _minioSettings;
    private readonly IOrderRepository _orderRepository;

    public CartService(
        IProductRepository productRepository,
        IOptions<MinioSettings> options,
        UserManager<User> userManager,
        IOrderRepository orderRepository)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _minioSettings = options.Value ?? throw new ArgumentNullException(nameof(options));
        _userManager = userManager;
        _orderRepository = orderRepository;
    }

    public async Task<Dictionary<ProductDto, int>> GetListAsync(Dictionary<int, int> cartItems)
    {
        var ids = cartItems.Select(x => x.Key).ToList();

        var query = await _productRepository
            .Queryable
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id))
            .Select(p => new ProductDto()
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                Price = p.Price,
                ImageUrl = $"{_minioSettings.Endpoint}/{_minioSettings.BucketName}/{p.ImageUrl}",
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
            }).ToListAsync();

        var result = query.ToDictionary(
            product => product,
            product => cartItems[product.Id]
        );

        return result;
    }

    public async Task<bool> CheckoutCartAsync(int userId, Dictionary<int, int> cartItems)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return false;
        }
        // Create order detail
        var ids = cartItems.Select(x => x.Key).ToList();

        var products = await _productRepository
            .Queryable
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id))
            .Select(p => new ProductDto()
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CategoryId = p.CategoryId,
            }).ToListAsync();

        var orderDetails = new HashSet<OrderDetail>();

        var total = 0;

        foreach (var item in products)
        {
            var orderDetail = new OrderDetail()
            {
                ProductId = item.Id,
                ProductName = item.Name,
                Price = item.Price,
                Quantity = cartItems[item.Id]
            };

            total += orderDetail.Price * orderDetail.Quantity;
            orderDetails.Add(orderDetail);
        }
        // create order
        var order = new Order()
        {
            UserId = userId,
            TotalAmount = total,
            Note = "",
            OrderDetails = orderDetails
        };

        await _orderRepository.AddAsync(order);
        await _orderRepository.UnitOfWork.SaveChangesAsync();

        return true;
    }
}