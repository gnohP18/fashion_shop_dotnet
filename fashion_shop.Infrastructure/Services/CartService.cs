using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.DTOs.Common;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.DTOs.Responses.Admin;
using fashion_shop.Core.DTOs.Responses.User;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.Core.Interfaces.Services;
using fashion_shop.Infrastructure.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace fashion_shop.Infrastructure.Services;

public class CartService : ICartService
{
    private readonly IProductRepository _productRepository;
    private readonly IProductItemRepository _productItemRepository;
    private readonly UserManager<User> _userManager;
    private readonly MinioSettings _minioSettings;
    private readonly IOrderRepository _orderRepository;

    public CartService(
        IProductRepository productRepository,
        IOptions<MinioSettings> options,
        UserManager<User> userManager,
        IOrderRepository orderRepository,
        IProductItemRepository productItemRepository)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _minioSettings = options.Value ?? throw new ArgumentNullException(nameof(options));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _productItemRepository = productItemRepository ?? throw new ArgumentNullException(nameof(productItemRepository));
    }

    public async Task<Dictionary<CartItemDto, int>> GetListAsync(Dictionary<int, int> cartItems)
    {
        var ids = cartItems.Select(x => x.Key).ToList();

        var query = await _productItemRepository
            .Queryable
            .AsNoTracking()
            .Include(pi => pi.Product)
            .Where(x => ids.Contains(x.Id))
            .Select(p => new CartItemDto()
            {
                Id = p.Id,
                ProductName = p.Product.Name,
                ProductSlug = p.Product.Slug,
                Code = p.Code,
                Price = p.Price,
                ImageUrl = $"{_minioSettings.Endpoint}/{_minioSettings.BucketName}/{p.ImageUrl}",
                CategoryId = p.Product.CategoryId,
                CategoryName = p.Product.Category.Name,
                IsVariant = p.Product.IsVariant,
                VariantObjects = p.VariantObjects
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

        var products = await _productItemRepository
            .Queryable
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id))
            .Select(p => new
            {
                Id = p.Id,
                ProductId = p.Product.Id,
                Name = p.Product.Name,
                Price = p.Price,
                CategoryId = p.Product.CategoryId,
            }).ToListAsync();

        var orderDetails = new HashSet<OrderDetail>();

        var total = 0;

        foreach (var item in products)
        {
            var orderDetail = new OrderDetail()
            {
                ProductItemId = item.Id,
                ProductId = item.ProductId,
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

    public async Task<bool> CheckProductExistByProductItemIdAsync(int productItemId)
    {
        return await _productItemRepository
            .Queryable
            .AsNoTracking()
            .WithoutDeleted()
            .Where(pi => pi.Id == productItemId && pi.Product.IsDeleted == false)
            .AnyAsync();
    }
}