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
using fashion_shop.API.Attributes;

namespace fashion_shop.API.Controllers.Admin
{
    [ApiController]
    [Authenticate]
    [Tags("Product Management")]
    [Route("api/product-management")]
    public class ProductManagementController : APIController<ProductManagementController>
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IProductItemService _productItemService;
        private readonly IMediaFileService _mediaFileService;

        public ProductManagementController(
            ILogger<ProductManagementController> logger,
            ICategoryService categoryService,
            IProductService productService,
            IMediaFileService mediaFileService,
            IProductItemService productItemService) : base(logger)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _mediaFileService = mediaFileService ?? throw new ArgumentNullException(nameof(mediaFileService));
            _productItemService = productItemService ?? throw new ArgumentNullException(nameof(productItemService));
        }

        #region Categories

        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ErrorResponse<CreateCategoryResponse>("Validation failed");
            }

            var data = await _categoryService.CreateAsync(request);

            return CreatedResponse<string>("Created successfully");
        }

        [HttpGet("categories")]
        public async Task<PaginationData<CategoryDto>> GetListCategory([FromQuery] GetCategoryRequest request)
        {
            return await _categoryService.GetListAsync(request);
        }

        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategoryAsync(int id)
        {
            await _categoryService.DeleteAsync(id);

            return NoContentResponse<string>("Deleted data successfully");
        }

        #endregion

        #region Basic Products
        [HttpGet("products")]
        public async Task<PaginationData<BasicProductDto>> GetProductAsync([FromQuery] GetProductRequest request)
        {
            return await _productService.GetListAsync(request);
        }

        [HttpPost("products")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ErrorResponse<string>("Validation Failed");
            }
            await _productService.CreateAsync(request);

            return CreatedResponse<string>("Created successfully");
        }

        /// <summary>
        /// Upload with server
        /// </summary>
        /// <param name="id">Product Id</param>
        /// <param name="request">Request file</param>
        /// <returns>Url</returns>
        /// <exception cref="NotFoundException">Not found Product</exception>
        [HttpPost("products/{id}/upload-image")]
        public async Task<IActionResult> UploadProductImage(int id, [FromForm] UploadImageRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ErrorResponse<string>("Validation Failed");
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

            return NoContentResponse<string>("Upload file successfully"); ;
        }

        /// <summary>
        /// Upload directly in to Minio
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("products/{id}/presigned-upload")]
        public async Task<IActionResult> CreatePresignUploadBasicProductUrl(int id, [FromBody] CreatePresignedUrlRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ErrorResponse<string>("Validation Failed");
            }

            var product = await _productService.GetDetailAsync(id);

            if (product is null)
            {
                throw new NotFoundException($"Not found product={id}");
            }

            var presignUrl = await _mediaFileService
                .CreatePresignedUrlAsync(request, nameof(Product).ToLower(), product.Id);

            return OkResponse<string>(presignUrl, "Created successfully");
        }

        [HttpPut("products/{id}")]
        public async Task<IActionResult> UpdateBasicProduct(int id, [FromBody] UpdateProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ErrorResponse<string>("Validation Failed");
            }

            var product = await _productService.GetDetailAsync(id);

            if (product is null)
            {
                throw new NotFoundException($"Not found product={id}");
            }

            await _productService.UpdateBasicInfoAsync(id, request);

            return OkResponse<string>(string.Empty, "Update successfully");
        }

        [HttpGet("products/{id}")]
        public async Task<IActionResult> GetProductDetail(int id)
        {
            var data = await _productService.GetDetailAsync(id);

            if (data is null)
            {
                throw new NotFoundException($"Not found product Id={id}");
            }

            return OkResponse<ProductDto>(data, "Get data successfully");
        }

        [HttpDelete("products/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteAsync(id);

            return NoContentResponse<string>("Deleted data successfully");
        }
        #endregion

        #region Variant & Item Products
        [HttpPut("products/variants/{id}")]
        public async Task<IActionResult> UpdateProductVariant(int id, [FromBody] UpdateProductVariantRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ErrorResponse<string>("Validation Failed");
            }

            var product = await _productService.GetDetailAsync(id);

            if (product is null)
            {
                throw new NotFoundException($"Not found product={id}");
            }

            if (request.IsVariant)
            {
                if (product.IsVariant && request.ProductItems.Count == 0)
                {
                    throw new BadRequestException("Missing ProductItem Array");
                }

                if (!product.IsVariant && (request.ProductVariants.Count == 0 || request.Variants.Count == 0))
                {
                    throw new BadRequestException("Missing ProductVariants Array Or Variants Array");
                }
            }

            await _productService.UpdateProductVariantAsync(id, request);

            return OkResponse<string>(string.Empty, "Update successfully");
        }

        [HttpPost("products/product-items/{id}/presigned-upload")]
        public async Task<IActionResult> CreatePresignUploadProductItemUrl(int id, [FromBody] CreatePresignedUrlRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ErrorResponse<string>("Validation Failed");
            }

            var productItem = await _productItemService.GetDetailAsync(id);

            if (productItem is null)
            {
                throw new NotFoundException($"Not found product item={id}");
            }

            var presignUrl = await _mediaFileService
                .CreatePresignedUrlAsync(request, nameof(ProductItem).ToLower(), productItem.Id);

            return OkResponse<string>(presignUrl, "Created successfully");
        }
    }
    #endregion
}