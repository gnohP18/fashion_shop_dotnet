using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using fashion_shop.MVC.Models;
using System.Threading.Tasks;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.Common;
using fashion_shop.Core.Interfaces.Services;
using fashion_shop.Core.DTOs.Common;
using Microsoft.Extensions.Options;

namespace fashion_shop.MVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly MinioSettings _minioSettings;

    public class HomeViewModel
    {
        public List<ProductDto> FeatureProducts { get; set; } = new List<ProductDto>();
    }

    public HomeController(
        ILogger<HomeController> logger,
        IProductService productService,
        IOptions<MinioSettings> options,
        ICategoryService categoryService)
    {
        _logger = logger;
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        _minioSettings = options.Value;
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index()
    {
        // Category
        var categoryGetParameter = new GetCategoryRequest();
        var categories = (await _categoryService.GetListAsync(categoryGetParameter)).Data;

        // Feature Product
        var featureProductParams = new GetProductRequest()
        {
            Offset = 5,
            Page = 1,
            SortBy = "created_at",
            Direction = PaginationConstant.DefaultSortDirection
        };

        var paginationFeatureData = (await _productService.GetListAsync(featureProductParams)).Data.ToList();

        // Collection Product
        var categoryIds = categories.Select(c => c.Id).ToArray();

        var random = new Random();

        var randomCategoryCollectionId = categoryIds[random.Next(categoryIds.Length)];
        var collectionProductParams = new GetProductRequest()
        {
            CategorySlug = categories.First(c => c.Id == randomCategoryCollectionId).Slug,
            Offset = 2
        };
        var paginationCollectionData = (await _productService.GetListAsync(collectionProductParams)).Data;

        ViewBag.FeatureProducts = paginationFeatureData;
        ViewBag.CollectionProducts = paginationCollectionData;

        return View();
    }



    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
