using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.Common;
using fashion_shop.Core.DTOs.Requests.User;
using fashion_shop.Core.DTOs.Responses.User;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace fashion_shop.MVC.Controllers
{
    [Route("profile")]
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly IOrderService _orderService;
        private readonly ISettingService _settingService;
        private readonly UserManager<User> _userManager;

        public ProfileController(
            ILogger<ProfileController> logger,
            IOrderService orderService,
            UserManager<User> userManager,
            ISettingService settingService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
        }

        [HttpGet("index")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("order-detail")]
        public async Task<IActionResult> OrderDetail(int id)
        {
            var order = await _orderService.GetOrderAsync(id);

            if (order is null)
            {
                return RedirectToAction("History", "Profile");
            }

            var orderDetail = await _orderService.GetOrderDetailAsync(order);
            var user = await _userManager.GetUserAsync(User);

            ViewBag.OrderDetail = orderDetail;
            ViewBag.Profile = await _userManager.GetUserAsync(User);
            ViewBag.BasicInfo = await _settingService.GetSettingAsync<BasicInfoSettingResponse>(SettingPrefixConstants.BasicInfoPrefix);

            return View();
        }

        [HttpGet("history")]
        public async Task<IActionResult> History(GetHistoryOrderRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return RedirectToAction("Indetity", "Login");
            }
            var paginationOrderData = await _orderService.GetHistoryOrderAsync(user.Id, request);

            ViewBag.OrderData = paginationOrderData.Data;
            ViewBag.Meta = new
            {
                CurrentPage = paginationOrderData.CurrentPage,
                PageSize = paginationOrderData.PageSize,
                Total = paginationOrderData.Total,
                LastPage = paginationOrderData.LastPage,
            };

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}