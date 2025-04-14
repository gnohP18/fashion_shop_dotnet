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
    private readonly MinioSettings _minioSettings;

    public class HomeViewModel
    {
        public List<ProductDto> FeatureProducts { get; set; } = new List<ProductDto>();
    }

    public HomeController(ILogger<HomeController> logger, IProductService productService, IOptions<MinioSettings> options)
    {
        _logger = logger;
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        _minioSettings = options.Value;
    }

    public async Task<IActionResult> Index()
    {
        var featureProductParams = new GetProductRequest()
        {
            Offset = 6,
            Page = 1,
            SortBy = "created_at",
            Direction = PaginationConstant.DefaultSortDirection
        };

        var paginationData = (await _productService.GetListAsync(featureProductParams)).Data.ToList();

        var latestProductParams = new GetProductRequest()
        {
            Offset = 6,
            Page = 1,
            SortBy = "created_at",
            Direction = PaginationConstant.DefaultSortDirection
        };

        var viewModel = new HomeViewModel
        {
            FeatureProducts = paginationData
        };

        return View(viewModel);
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
