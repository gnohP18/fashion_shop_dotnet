using fashion_shop.API.Attributes;
using fashion_shop.Core.Common;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.DTOs.Responses;
using fashion_shop.Core.DTOs.Responses.Admin;
using fashion_shop.Core.DTOs.Responses.User;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Exceptions;
using fashion_shop.Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace fashion_shop.API.Controllers.Admin
{
    [ApiController]
    [Authenticate]
    [Tags("Order Management")]
    [Route("api/order-management")]
    public class OrderManagementController : APIController<OrderManagementController>
    {
        private readonly IOrderService _orderService;

        public OrderManagementController(
            ILogger<OrderManagementController> logger,
            IOrderService orderService,
            IHttpContextAccessor httpContextAccessor,
            UserManager<User> userManager
            ) : base(logger, httpContextAccessor, userManager)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        [HttpGet("")]
        public async Task<PaginationData<BasicOrderResponse>> GetList([FromQuery] GetListOrderRequest request)
        {
            return await _orderService.GetListOrder(request);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetailOrder(int id)
        {
            var order = await _orderService.GetOrderAsync(id);

            if (order is null)
            {
                throw new NotFoundException($"Not found order Id={id}");
            }

            var data = await _orderService.GetOrderDetailAsync(order);

            return OkResponse<OrderDetailResponse>(data, "Get data successfully");
        }

        [HttpPost("create-by-admin")]
        public async Task<IActionResult> CreateOrderByAdmin([FromBody] CreateOrderByAdminRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ErrorResponse<string>("Validation failed");
            }

            await _orderService.CreateOrderByAdminAsync(request);

            return NoContentResponse<string>("Created successfully");
        }

        [HttpGet("get-product-options")]
        public async Task<List<DropdownResponse>> GetProductOptions()
        {
            return await _orderService.GetProductOptionsAsync();
        }
    }
}