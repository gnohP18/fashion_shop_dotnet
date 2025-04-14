using System.Net;
using fashion_shop.API.ExternalService.DTOs;
using fashion_shop.Core.DTOs;
using fashion_shop.Core.DTOs.Common;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.DTOs.Responses;
using fashion_shop.Core.DTOs.Responses.Admin;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Interfaces.Services;
using fashion_shop.Core.Exceptions;
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
        private readonly IMediaFileService _mediaFileService;

        public ProductManagementController(
            ILogger<ProductManagementController> logger,
            ICategoryService categoryService,
            IProductService productService,
            IMediaFileService mediaFileService) : base(logger)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _mediaFileService = mediaFileService ?? throw new ArgumentNullException(nameof(mediaFileService));
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

        /// <summary>
        /// Upload with server
        /// </summary>
        /// <param name="id">Product Id</param>
        /// <param name="request">Request file</param>
        /// <returns>Url</returns>
        /// <exception cref="NotFoundException">Not found Product</exception>
        [HttpPost("products/{id}/upload-image")]
        public async Task<BaseResponse<string>> UploadProductImage(int id, [FromForm] UploadImageRequest request)
        {
            if (!ModelState.IsValid)
            {
                return HandleInvalidModel<string>();
            }

            var product = await _productService.GetDetailAsync(id);

            if (product is null)
            {
                throw new NotFoundException($"Not found product={id}");
            }

            var data = await _mediaFileService.UploadFileAsync(new CreateMediaFileRequest()
            {
                FileStream = request.File.OpenReadStream(),
                FileExtension = request.File.ContentType.Split('/')[1],
                FileName = request.File.FileName,
                ContentType = request.File.ContentType,
                ObjectType = nameof(Product).ToLower(),
                ObjectId = id
            });

            return new BaseResponse<string>()
            {
                StatusCode = HttpStatusCode.OK,
                Data = data,
            };
        }

        /// <summary>
        /// Upload directly in to Minio
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("products/{id}/presigned-upload")]
        public async Task<BaseResponse<string>> CreatePresignUploadUrl(int id, [FromBody] CreatePresignedUrlRequest request)
        {
            if (!ModelState.IsValid)
            {
                return HandleInvalidModel<string>();
            }

            var product = await _productService.GetDetailAsync(id);

            if (product is null)
            {
                throw new NotFoundException($"Not found product={id}");
            }

            var presignUrl = await _mediaFileService
                .CreatePresignedUrlAsync(request, nameof(Product).ToLower(), product.Id);

            return new BaseResponse<string>()
            {
                Data = presignUrl
            };
        }

        [HttpGet("products/{id}")]
        public async Task<BaseResponse<ProductDto>> GetProductDetail(int id)
        {
            var data = await _productService.GetDetailAsync(id);

            if (data is null)
            {
                throw new NotFoundException($"Not found product Id={id}");
            }

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