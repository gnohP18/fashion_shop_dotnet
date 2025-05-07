using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using fashion_shop.API.ExternalService.Entities;
using fashion_shop.Core.Common;
using fashion_shop.Core.DTOs.Common;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.DTOs.Responses.Admin;
using fashion_shop.Core.DTOs.Responses.User;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.Core.Interfaces.Services;
using fashion_shop.Infrastructure.Common;
using Firebase.Database;
using Firebase.Database.Query;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace fashion_shop.Infrastructure.Services;

public class CartService : ICartService
{
    private readonly IProductRepository _productRepository;
    private readonly IProductItemRepository _productItemRepository;
    private readonly UserManager<User> _userManager;
    private readonly MinioSettings _minioSettings;
    private readonly IOrderRepository _orderRepository;
    private readonly FirebaseClient _firebaseClient;
    private readonly FirebaseMessaging _messaging;
    private readonly ICurrenUserContext _currenUserContext;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IDatabase _redis;


    public CartService(
        IProductRepository productRepository,
        IOptions<MinioSettings> options,
        UserManager<User> userManager,
        IOrderRepository orderRepository,
        IProductItemRepository productItemRepository,
        FirebaseMessaging messaging,
        IOptions<FirebaseSettings> optionsFirebase,
        ICurrenUserContext currenUserContext,
        IConnectionMultiplexer connectionMultiplexer)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _minioSettings = options.Value ?? throw new ArgumentNullException(nameof(options));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _productItemRepository = productItemRepository ?? throw new ArgumentNullException(nameof(productItemRepository));
        _messaging = messaging ?? throw new ArgumentNullException(nameof(messaging));
        _firebaseClient = new FirebaseClient(optionsFirebase.Value.DefaultConnection) ?? throw new ArgumentNullException(nameof(optionsFirebase.Value));
        _currenUserContext = currenUserContext ?? throw new ArgumentNullException(nameof(optionsFirebase.Value));
        _redis = connectionMultiplexer.GetDatabase() ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
        _connectionMultiplexer = connectionMultiplexer;
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
                ImageUrl = !string.IsNullOrEmpty(p.ImageUrl) ? _minioSettings.GetUrlImage(p.ImageUrl, false, false) : "",
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
        var order = new Core.Entities.Order()
        {
            UserId = userId,
            TotalAmount = total,
            Note = "",
            OrderDetails = orderDetails
        };

        await _orderRepository.AddAsync(order);
        await _orderRepository.UnitOfWork.SaveChangesAsync();

        var message = new AdminMessage
        {
            Title = "Đơn hàng mới",
            Body = $"Đơn hàng mới trị giá {Core.Common.Function.FormatVnd(order.TotalAmount)}đ vừa được tạo",
            IsRead = false,
            RedirectUrl = $"http://localhost:3000/order/{order.Id}",
            CreatedAt = order.CreatedAt,
            SenderId = 0,
            ReceiverId = order.UserId
        };

        await SendMessageToAdmin(message);

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

    private async Task SendMessageToAdmin(AdminMessage tempMessage)
    {
        var admins = await _userManager.GetUsersInRoleAsync("Admin");
        var manager = await _userManager.GetUsersInRoleAsync("Manager");

        foreach (var admin in admins.Concat(manager))
        {
            var newMessgae = tempMessage;

            newMessgae.SenderId = admin.Id;
            await _firebaseClient
                .Child("notifications")
                .Child(admin.Id.ToString())
                .PostAsync(newMessgae);
        }

        var pattern = $"{AuthConstant.FCM_TOKEN_LIST}*";

        var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First());

        var tokenKeys = server.Keys(pattern: pattern).ToList();

        var messages = new List<Message>();

        foreach (var key in tokenKeys)
        {
            var token = await _redis.StringGetAsync(key);

            if (token.HasValue)
            {
                messages.Add(new Message
                {
                    Token = token.ToString(),
                    Notification = new Notification
                    {
                        Title = tempMessage.Title,
                        Body = tempMessage.Body
                    }
                });
            }
        }

        if (messages.Any())
        {
            await _messaging.SendEachAsync(messages);
        }
        else
        {
            Console.WriteLine("No messages to send.");
        }
    }
}