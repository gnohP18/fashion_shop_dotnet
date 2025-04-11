using System.Net;
using fashion_shop.Core.DTOs;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.DTOs.Responses;
using fashion_shop.Core.DTOs.Responses.Admin;
using fashion_shop.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace fashion_shop.API.Controllers.Admin
{
    [ApiController]
    [Tags("Product Management")]
    [Route("api/product-management")]
    public class ProductManagementController : APIController<ProductManagementController>
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;

        public ProductManagementController(
            ILogger<ProductManagementController> logger,
            ICategoryService categoryService,
            IProductService productService) : base(logger)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        [HttpPost("categories")]
        public async Task<BaseResponse<CreateCategoryResponse>> CreateCategory([FromBody] CreateCategoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                return HandleInvalidModel<CreateCategoryResponse>();
            }
            var data = await _categoryService.CreateAsync(request);

            return new BaseResponse<CreateCategoryResponse>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Created successfully",
                Data = data,
            };
        }

        [HttpGet("categories")]
        public async Task<PaginationData<CategoryDto>> GetListCategory([FromQuery] GetCategoryRequest request)
        {
            return await _categoryService.GetListAsync(request);
        }

        [HttpDelete("categories/{id}")]
        public async Task<BaseResponse<string>> DeleteCategoryAsync(int id)
        {
            await _categoryService.DeleteAsync(id);

            return new BaseResponse<string>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Deleted data successfully",
            };
        }

        [HttpGet("products")]
        public async Task<PaginationData<ProductDto>> GetProductAsync([FromQuery] GetProductRequest request)
        {
            return await _productService.GetListAsync(request);
        }

        [HttpPost("products")]
        public async Task<BaseResponse<CreateProductResponse>> CreateProduct([FromBody] CreateProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                return HandleInvalidModel<CreateProductResponse>();
            }
            var data = await _productService.CreateAsync(request);

            return new BaseResponse<CreateProductResponse>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Created successfully",
                Data = data,
            };
        }

        [HttpGet("products/{id}")]
        public async Task<BaseResponse<ProductDto>> GetProductDetail(int id)
        {
            var data = await _productService.GetDetailAsync(id);

            return new BaseResponse<ProductDto>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Get data successfully",
                Data = data,
            };
        }

        [HttpDelete("products/{id}")]
        public async Task<BaseResponse<string>> DeleteProduct(int id)
        {
            await _productService.DeleteAsync(id);

            return new BaseResponse<string>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Deleted data successfully",
            };
        }
    }
}