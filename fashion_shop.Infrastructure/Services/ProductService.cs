using AutoMapper;
using fashion_shop.Core.Common;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.DTOs.Responses;
using fashion_shop.Core.DTOs.Responses.Admin;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.Core.Interfaces.Services;
using fashion_shop.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using fashion_shop.Core.DTOs.Common;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using fashion_shop.Infrastructure.Common;

namespace fashion_shop.Infrastructure.Services;

public partial class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IProductVariantRepository _productVariantRepository;
    private readonly IVariantRepository _variantRepository;
    private readonly IProductItemRepository _productItemRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IOrderDetailRepository _orderDetailRepository;
    private readonly IMapper _mapper;
    private readonly MinioSettings _minioSettings;
    private readonly ILogger _logger;

    public ProductService(
        IProductRepository productRepository,
        IMapper mapper,
        IOptions<MinioSettings> options,
        ICategoryRepository categoryRepository,
        IOrderDetailRepository orderDetailRepository,
        ILogger<ProductService> logger,
        IProductVariantRepository productVariantRepository,
        IVariantRepository variantRepository,
        IProductItemRepository productItemRepository)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(_mapper));
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        _minioSettings = options.Value ?? throw new ArgumentNullException(nameof(options));
        _orderDetailRepository = orderDetailRepository ?? throw new ArgumentNullException(nameof(orderDetailRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _productVariantRepository = productVariantRepository ?? throw new ArgumentNullException(nameof(productVariantRepository));
        _variantRepository = variantRepository ?? throw new ArgumentNullException(nameof(variantRepository));
        _productItemRepository = productItemRepository ?? throw new ArgumentNullException(nameof(_productItemRepository));
    }

    public async Task<PaginationData<BasicProductDto>> GetListAsync(GetProductRequest request)
    {
        var query = _productRepository.Queryable
            .WithoutDeleted()
            .AsNoTracking();

        if (!string.IsNullOrEmpty(request.KeySearch))
        {
            query = query.Where(
                x => EF.Functions.ILike(x.Name, $"%{request.KeySearch}%") ||
                    EF.Functions.ILike(x.Slug, request.KeySearch));
        }

        if (!string.IsNullOrWhiteSpace(request.CategorySlug))
        {
            query = query.Where(p => p.Category.Slug == request.CategorySlug.ToLower());
        }

        var sortByField = !string.IsNullOrEmpty(request.SortBy) ? request.SortBy : PaginationConstant.DefaultSortKey;

        query = OrderByCondition(query, sortByField, request.Direction.ToUpper() == PaginationConstant.DefaultSortDirection);

        query = query.Include(x => x.Category);

        var data = await query
            .Skip((request.Page - 1) * request.Offset)
            .Take(request.Offset)
            .Select(p => new BasicProductDto()
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                Price = p.Price,
                ImageUrl = !string.IsNullOrWhiteSpace(p.ImageUrl) ?
                    $"http://{_minioSettings.Endpoint}/{_minioSettings.BucketName}/{p.ImageUrl}" : ProductConstant.DefaultImage600,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                IsVariant = p.IsVariant
            })
            .ToListAsync();

        var total = await query.Select(x => x.Id).CountAsync();

        return new PaginationData<BasicProductDto>(data, request.Offset, request.Page, total);
    }

    public async Task<ProductDto?> GetDetailAsync(int id)
    {
        var productDto = await _productRepository.Queryable
            .AsNoTracking()
            .WithoutDeleted()
            .Select(p => new ProductDto()
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                Price = p.Price,
                ImageUrl = !string.IsNullOrWhiteSpace(p.ImageUrl)
                    ? $"http://{_minioSettings.Endpoint}/{_minioSettings.BucketName}/{p.ImageUrl}" : ProductConstant.DefaultImage600,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                IsVariant = p.IsVariant,
                Description = p.Description,
                ProductVariants = p.ProductVariants
                    .AsQueryable()
                    .AsNoTracking()
                    .Where(pv => !pv.IsDeleted)
                    .Select(pv => new ProductVariantDto
                    {
                        Name = pv.Name,
                        Priority = pv.Priority,
                        Variants = pv.Variants
                                .AsQueryable()
                                .AsNoTracking()
                                .Where(v => !v.IsDeleted)
                                .Select(v => new VariantDto
                                {
                                    Code = v.Code,
                                    Value = v.Value
                                }).ToList()
                    }).ToList(),
                ProductItems = p.ProductItems
                .AsQueryable()
                .AsNoTracking()
                .Where(i => !i.IsDeleted)
                .Select(i => new ProductItemDto()
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    Code = i.Code,
                    Price = i.Price,
                    ImageUrl = !string.IsNullOrEmpty(i.ImageUrl) ?
                        $"http://{_minioSettings.Endpoint}/{_minioSettings.BucketName}/{i.ImageUrl}" : ProductConstant.DefaultImage600,
                }).ToList()
            }).FirstOrDefaultAsync(p => p.Id == id);

        return productDto ?? null;
    }

    public async Task DeleteAsync(int id)
    {
        var product = _productRepository
            .Queryable
            .WithoutDeleted()
            .FirstOrDefault(_ => _.Id == id);

        if (product is null)
        {
            throw new NotFoundException($"Not found product id={id}");
        }

        await DisableProductVariant(id);

        _productRepository.Delete(product);
        await _productRepository.UnitOfWork.SaveChangesAsync();
    }

    public async Task<ProductDto?> GetDetailBySlugAsync(string slug)
    {
        return await _productRepository
            .Queryable
            .WithoutDeleted()
            .Select(p => new ProductDto()
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                Price = p.Price,
                ImageUrl = !string.IsNullOrWhiteSpace(p.ImageUrl)
                    ? $"http://{_minioSettings.Endpoint}/{_minioSettings.BucketName}/{p.ImageUrl}" : ProductConstant.DefaultImage600,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                IsVariant = p.IsVariant,
                Description = p.Description,
                ProductVariants = p.ProductVariants
                    .AsQueryable()
                    .AsNoTracking()
                    .Where(pv => !pv.IsDeleted)
                    .Select(pv => new ProductVariantDto
                    {
                        Name = pv.Name,
                        Priority = pv.Priority,
                        Variants = pv.Variants
                                .AsQueryable()
                                .AsNoTracking()
                                .Where(v => !v.IsDeleted)
                                .Select(v => new VariantDto
                                {
                                    Code = v.Code,
                                    Value = v.Value
                                }).ToList()
                    }).ToList(),
                ProductItems = p.ProductItems
                .AsQueryable()
                .AsNoTracking()
                .Where(i => !i.IsDeleted)
                .Select(i => new ProductItemDto()
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    Code = i.Code,
                    Price = i.Price,
                    ImageUrl = !string.IsNullOrEmpty(i.ImageUrl) ?
                        $"http://{_minioSettings.Endpoint}/{_minioSettings.BucketName}/{i.ImageUrl}" : ProductConstant.DefaultImage600,
                }).ToList()
            }).FirstOrDefaultAsync(p => p.Slug == slug.ToLower());
    }

    private IQueryable<Product> OrderByCondition(IQueryable<Product> source, string field, bool isDescending)
    {
        return field.ToLower() switch
        {
            "id" => isDescending ? source.OrderByDescending(p => p.Id) : source.OrderBy(p => p.Id),
            "created_at" => isDescending ? source.OrderByDescending(p => p.CreatedAt) : source.OrderBy(p => p.CreatedAt),
            "price" => isDescending ? source.OrderByDescending(p => p.Price) : source.OrderBy(p => p.Price),
            "name" => isDescending ? source.OrderByDescending(p => p.Name) : source.OrderBy(p => p.Name),
            _ => source
        };
    }
}
