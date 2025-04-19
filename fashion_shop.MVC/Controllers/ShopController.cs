using fashion_shop.Core.Common;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace fashion_shop.MVC.Controllers;

[Route("shop")]
public class ShopController : Controller
{
    private readonly ILogger<ShopController> _logger;
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public class DetailViewModel
    {
        public ProductDto ProductDetail { get; set; } = null!;
    }

    public ShopController(
        ILogger<ShopController> logger,
        IProductService productService,
        ICategoryService categoryService)
    {
        _logger = logger;
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
    }

    [HttpGet("index")]
    public async Task<IActionResult> Index(GetProductRequest request)
    {
        // Category
        var categoryGetParameter = new GetCategoryRequest();

        var categories = (await _categoryService.GetListAsync(categoryGetParameter)).Data;

        request.Offset = request.Offset != PaginationConstant.PageSize ? request.Offset : 6;
        var paginationProducts = await _productService.GetListAsync(request);

        var products = paginationProducts.Data;

        ViewBag.Categories = categories;
        ViewBag.Products = products;
        ViewBag.Meta = new
        {
            CurrentPage = paginationProducts.CurrentPage,
            PageSize = paginationProducts.PageSize,
            Total = paginationProducts.Total,
            LastPage = paginationProducts.LastPage,
        };

        return View();
    }

    [HttpGet("detail/{slug}")]
    public async Task<IActionResult> Detail(string slug)
    {
        var product = await _productService.GetDetailBySlugAsync(slug);

        if (product is null)
        {
            return NotFound();
        }

        ViewBag.ProductDetail = product;

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View("Error!");
    }
}