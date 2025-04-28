using fashion_shop.Core.Common;
using fashion_shop.Core.DTOs.Common;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.DTOs.Requests.User;
using fashion_shop.Core.DTOs.Responses;
using fashion_shop.Core.DTOs.Responses.Admin;
using fashion_shop.Core.DTOs.Responses.User;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Exceptions;
using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.Core.Interfaces.Services;
using fashion_shop.Infrastructure.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Order = fashion_shop.Core.Entities.Order;

namespace fashion_shop.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductItemRepository _productItemRepository;
        private readonly MinioSettings _minioSettings;
        private readonly UserManager<User> _userManager;
        private readonly IDatabase _redis;

        public OrderService(
            IOrderRepository orderRepository,
            IOptions<MinioSettings> options,
            IOrderDetailRepository orderDetailRepository,
            UserManager<User> userManager,
            IProductItemRepository productItemRepository,
            IProductRepository productRepository,
            IConnectionMultiplexer connectionMultiplexer)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _orderDetailRepository = orderDetailRepository ?? throw new ArgumentNullException(nameof(orderDetailRepository));
            _minioSettings = options.Value ?? throw new ArgumentNullException(nameof(options));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _productItemRepository = productItemRepository ?? throw new ArgumentNullException(nameof(productItemRepository));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _redis = connectionMultiplexer.GetDatabase() ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
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

        public async Task<PaginationData<BasicOrderResponse>> GetListOrder(GetListOrderRequest request)
        {
            var query = _orderRepository.Queryable.AsNoTracking();

            if (!string.IsNullOrEmpty(request.KeySearch))
            {
                query = query.Where(
                    x => EF.Functions.ILike(x.User.UserName ?? "", $"%{request.KeySearch}%"));
            }

            if (request.MinDate is not null)
            {
                query = query.Where(o => o.CreatedAt >= request.MinDate);
            }

            if (request.MaxDate is not null)
            {
                query = query.Where(o => o.CreatedAt <= request.MaxDate);
            }

            if (request.MinPrice is not null)
            {
                query = query.Where(o => o.TotalAmount >= request.MinPrice);
            }

            if (request.MaxPrice is not null)
            {
                query = query.Where(o => o.TotalAmount <= request.MaxPrice);
            }

            var sortByField = (request.SortBy ?? PaginationConstant.DefaultSortKey).ToLower();

            query = OrderByCondition(query, sortByField, request.Direction.ToUpper() == PaginationConstant.DefaultSortDirection);

            var total = query.Count();

            var data = await query
                .Skip((request.Page - 1) * request.Offset)
                .Take(request.Offset)
                .Select(o => new BasicOrderResponse()
                {
                    Id = o.Id,
                    TotalAmount = o.TotalAmount,
                    UserId = o.User.Id,
                    Username = o.User.UserName ?? "",
                    TotalItem = o.OrderDetails.Count(),
                    CreatedAt = o.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                })
                .ToListAsync();

            return new PaginationData<BasicOrderResponse>(data, request.Offset, request.Page, total);
        }

        private IQueryable<Order> OrderByCondition(IQueryable<Order> source, string field, bool isDescending)
        {
            return field.ToLower() switch
            {
                "id" => isDescending ? source.OrderByDescending(p => p.Id) : source.OrderBy(p => p.Id),
                "createdat" => isDescending ? source.OrderByDescending(p => p.CreatedAt) : source.OrderBy(p => p.CreatedAt),
                "price" => isDescending ? source.OrderByDescending(p => p.TotalAmount) : source.OrderBy(p => p.TotalAmount),
                "name" => isDescending ? source.OrderByDescending(p => p.User.UserName) : source.OrderBy(p => p.User.UserName),
                _ => source
            };
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
                    VariantObjects = o.ProductItem.VariantObjects,
                    ProductItemId = o.ProductItemId,
                    ProductSlug = o.Product.Slug,
                    ProductName = o.ProductName,
                    Quantity = o.Quantity,
                    Price = o.Price,
                    ImageUrl = !string.IsNullOrWhiteSpace(o.ProductItem.ImageUrl) ? _minioSettings.GetUrlImage(o.ProductItem.ImageUrl, false, false) : ProductConstant.DefaultImage200
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


        public async Task CreateOrderByAdminAsync(CreateOrderByAdminRequest request)
        {
            // Check valid request
            if (request.OrderDetails.Count == 0)
            {
                throw new BadRequestException("Empty product item");
            }

            var productItemIds = request.OrderDetails.Select(od => od.ProductItemId);

            var productItems = await _productItemRepository
                .Queryable
                .AsNoTracking()
                .Where(x => productItemIds.Contains(x.Id))
                .Select(p => new
                {
                    Id = p.Id,
                    ProductId = p.Product.Id,
                    Name = p.Product.Name,
                    Price = p.Price,
                    CategoryId = p.Product.CategoryId,
                }).ToListAsync();

            if (productItemIds.Count() != productItems.Count())
            {
                throw new BadRequestException("Not found product");
            }

            var existUser = await _userManager
                .Users
                .Where(u => u.PhoneNumber == request.Phone)
                .FirstOrDefaultAsync();

            if (existUser is not null && request.IsGuest)
            {
                throw new BadRequestException($"Existed User with Phone number {request.Phone}");
            }

            var user = new User();

            // create new user
            if (request.IsGuest)
            {
                var now = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");

                user.Email = $"{UserConstant.GuestEmail}_{now}@tempMail.com";
                user.UserName = request.Phone;
                user.PhoneNumber = request.Phone;

                var createResult = await _userManager.CreateAsync(user, "180402");

                if (createResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");

                    var hashedPhone = Infrastructure.Common.Function.HashStringCRC32(request.Phone ?? "");

                    await _redis.StringSetBitAsync(RedisConstant.USER_PHONE_LIST, hashedPhone, true);
                    await _redis.StringSetBitAsync(RedisConstant.USER_USERNAME_LIST, hashedPhone, true);
                }
                else
                {
                    throw new UnprocessableContentException("Created user failed");
                }
            }
            else
            {
                user = existUser;
            }

            var orderDetails = new HashSet<OrderDetail>();

            var total = 0;

            foreach (var item in productItems)
            {
                var orderDetail = new OrderDetail()
                {
                    ProductItemId = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.Name,
                    Price = item.Price,
                    Quantity = request.OrderDetails
                        .Where(od => od.ProductItemId == item.Id)
                        .Select(pi => pi.Quantity)
                        .First()
                };

                total += orderDetail.Price * orderDetail.Quantity;
                orderDetails.Add(orderDetail);
            }

            // create order
            var order = new Order()
            {
                UserId = user?.Id ?? 0,
                TotalAmount = total,
                Note = request.Note,
                OrderDetails = orderDetails
            };

            await _orderRepository.AddAsync(order);
            await _orderRepository.UnitOfWork.SaveChangesAsync();
        }

        public async Task<List<DropdownResponse>> GetProductOptionsAsync()
        {
            return await _productRepository
                .Queryable
                .WithoutDeleted()
                .AsNoTracking()
                .Select(p => new DropdownResponse
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToListAsync();
        }
    }
}