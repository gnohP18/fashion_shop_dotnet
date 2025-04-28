using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.DTOs.Responses.Admin;
using fashion_shop.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace fashion_shop.API.Controllers.Admin
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : APIController<DashboardController>
    {
        private readonly IStatisticService _statisticService;

        public DashboardController(ILogger<DashboardController> logger, IStatisticService statisticService) : base(logger)
        {
            _statisticService = statisticService ?? throw new ArgumentNullException(nameof(statisticService));
        }

        [HttpGet("top-seller")]
        public async Task<List<TopSellerResponse>> GetTopSeller([FromQuery] GetTopSellerRequest request)
        {
            return await _statisticService.GetTopSellerAsync(request);
        }

        [HttpGet("sales-revenue")]
        public async Task<List<SalesRevenueResponse>> GetSalesRevenue([FromQuery] GetSalesRevenueRequest request)
        {
            return await _statisticService.GetSalesRevenueAsnc(request);
        }
    }
}