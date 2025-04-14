using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace fashion_shop.MVC.Controllers;

[Route("Shop")]
public class ShopController : Controller
{
    private readonly ILogger<ShopController> _logger;
    private readonly IProductService _productService;

    public class DetailViewModel
    {
        public ProductDto ProductDetail { get; set; } = null!;
    }

    public ShopController(ILogger<ShopController> logger, IProductService productService)
    {
        _logger = logger;
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
    }

    [HttpGet("index")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("detail/{id}")]
    public async Task<IActionResult> Detail(int id)
    {
        var product = await _productService.GetDetailAsync(id);

        ViewBag.ProductDetail = product;

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View("Error!");
    }
}