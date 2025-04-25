using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.DTOs.Responses;
using fashion_shop.Core.DTOs.Responses.Admin;
using fashion_shop.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace fashion_shop.API.Controllers.Admin
{
    [ApiController]
    [Route("api/order-management")]
    public class OrderManagementController : APIController<OrderManagementController>
    {
        private readonly IOrderService _orderService;

        public OrderManagementController(ILogger<OrderManagementController> logger, IOrderService orderService) : base(logger)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        [HttpGet("")]
        public async Task<PaginationData<BasicOrderResponse>> GetList([FromQuery] GetListOrderRequest request)
        {
            return await _orderService.GetListOrder(request);
        }
    }
}