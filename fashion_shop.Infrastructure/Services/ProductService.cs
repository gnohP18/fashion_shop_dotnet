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

namespace fashion_shop.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
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
        ILogger<ProductService> logger)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(_mapper));
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        _minioSettings = options.Value ?? throw new ArgumentNullException(nameof(options));
        _orderDetailRepository = orderDetailRepository ?? throw new ArgumentNullException(nameof(orderDetailRepository));
        _logger = logger;
    }

    public async Task<CreateProductResponse> CreateAsync(CreateProductRequest request)
    {
        var category = await _categoryRepository
            .Queryable
            .FirstOrDefaultAsync(_ => _.Id == request.CategoryId);

        if (category is null)
        {
            throw new NotFoundException($"Not found category Id={request.CategoryId}");
        }

        var product = new Product()
        {
            Name = request.Name,
            Slug = Function.GenerateSlugProduct(request.Slug),
            Price = request.Price,
            Description = request.Description,
            CategoryId = category.Id,
            IsVariant = request.IsVariant,
            ImageUrl = request.ImageUrl,
        };

        if (request.IsVariant && request.ProductVariants.Count > 0 && request.Variants.Count > 0)
        {
            // Handle create variant
            var productVariants = GenerateProductVariant(request.ProductVariants, request.Variants);

            product.ProductVariants = productVariants;

            var variantGroups = GenerateCombinations(productVariants.Select(p => p.Variants.Select(v => v.Code).ToList()).ToList());

            product.ProductItems = PrepareProductItemData(variantGroups);
            foreach (var item in product.ProductItems)
            {
                System.Console.WriteLine(item.Code);
            }
        }

        await _productRepository.AddAsync(product);
        await _productRepository.UnitOfWork.SaveChangesAsync();

        var dataMapping = _mapper.Map<CreateProductResponse>(product);

        return dataMapping;
    }

    public async Task<PaginationData<BasicProductDto>> GetListAsync(GetProductRequest request)
    {
        var query = _productRepository.Queryable.AsNoTracking();

        if (!string.IsNullOrEmpty(request.KeySearch))
        {
            query = query.Where(
                x => x.Name.ToLower() == request.KeySearch.ToLower() ||
                x.Slug.ToLower() == request.KeySearch.ToLower());
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
                ImageUrl = $"{_minioSettings.Endpoint}/{_minioSettings.BucketName}/{p.ImageUrl}",
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
            })
            .ToListAsync();

        var total = await query.Select(x => x.Id).CountAsync();

        return new PaginationData<BasicProductDto>(data, request.Offset, request.Page, total);
    }

    public async Task<ProductDto?> GetDetailAsync(int id)
    {
        var productDto = await _productRepository.Queryable
            .AsNoTracking()
            .Select(p => new ProductDto()
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                Price = p.Price,
                ImageUrl = $"{_minioSettings.Endpoint}/{_minioSettings.BucketName}/{p.ImageUrl}",
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                IsVariant = p.IsVariant,
                Description = p.Description,
                ProductVariants = p.ProductVariants.Select(pv => new ProductVariantDto
                {
                    Name = pv.Name,
                    Priority = pv.Priority,
                    Variants = pv.Variants.Select(v => new VariantDto
                    {
                        Code = v.Code,
                        Value = v.Value
                    }).ToList()
                }).ToList()
            }).FirstOrDefaultAsync(p => p.Id == id);
        System.Console.WriteLine(productDto?.IsVariant);
        return productDto ?? null;
    }

    public async Task DeleteAsync(int id)
    {
        var product = _productRepository.Queryable.FirstOrDefault(_ => _.Id == id);
        if (product is null)
        {
            throw new NotFoundException($"Not found product id={id}");
        }

        var canDelete = true;

        // Check if product have
        canDelete = !await _orderDetailRepository.Queryable.AsNoTracking().AnyAsync(p => p.ProductId == id);

        if (canDelete)
        {
            _productRepository.Delete(product);
            await _productRepository.UnitOfWork.SaveChangesAsync();
        }
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

    public async Task<ProductDto?> GetDetailBySlugAsync(string slug)
    {
        return await _productRepository.Queryable.Select(p => new ProductDto()
        {
            Id = p.Id,
            Name = p.Name,
            Slug = p.Slug,
            Price = p.Price,
            ImageUrl = $"{_minioSettings.Endpoint}/{_minioSettings.BucketName}/{p.ImageUrl}",
            CategoryId = p.CategoryId,
            CategoryName = p.Category.Name,
            IsVariant = p.IsVariant,
            Description = p.Description,
            ProductVariants = p.ProductVariants
                .AsQueryable()
                .AsNoTracking().Select(pv => new ProductVariantDto
                {
                    Name = pv.Name,
                    Priority = pv.Priority,
                    Variants = pv.Variants
                            .AsQueryable()
                            .AsNoTracking()
                            .Select(v => new VariantDto
                            {
                                Code = v.Code,
                                Value = v.Value
                            }).ToList()
                }).ToList(),
            ProductItems = p.ProductItems
                .AsQueryable()
                .AsNoTracking()
                .Where(p => p.Product.Slug == slug)
                .Select(i => new ProductItemDto()
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    Code = i.Code,
                    Price = i.Price,
                    ImageUrl = i.ImageUrl,
                }).ToList()
        }).FirstOrDefaultAsync(p => p.Slug == slug.ToLower());
    }

    private HashSet<ProductVariant> GenerateProductVariant(
        List<CreateProductVariantRequest> productVariants,
        List<CreateVariantRequest> variants)
    {
        productVariants = productVariants.OrderBy(p => p.Priority).ToList();

        var newProductVariants = new HashSet<ProductVariant>();

        productVariants.ForEach(productVariant =>
        {
            var selectedVariant = variants.Where(v => v.Priority == productVariant.Priority).ToList();

            var data = PrepareVariantData(selectedVariant);

            newProductVariants.Add(new ProductVariant()
            {
                Name = productVariant.Name,
                Priority = productVariant.Priority,
                Variants = data.ToHashSet()
            });
        });

        return newProductVariants;
    }

    private HashSet<Variant> PrepareVariantData(List<CreateVariantRequest> variants)
    {
        return variants.Select(v => new Variant()
        {
            Value = v.Value,
            Code = v.Value.ToLower()
        }).ToHashSet();
    }

    public static List<string> GenerateCombinations(List<List<string>> variantGroups)
    {
        var results = new List<string>();

        GenerateRecursive(variantGroups, 0, new List<string>(), results);

        return results;
    }

    /// <summary>
    /// Tạo đệ qui quay lui cho thêm list variant
    /// </summary>
    /// <param name="groups"></param>
    /// <param name="index"></param>
    /// <param name="current"></param>
    /// <param name="result"></param>
    private static void GenerateRecursive(
        List<List<string>> groups,
        int index,
        List<string> current,
        List<string> result)
    {
        if (index == groups.Count)
        {
            result.Add(string.Join("_", current));
            return;
        }

        foreach (var value in groups[index])
        {
            current.Add(value);
            GenerateRecursive(groups, index + 1, current, result);
            current.RemoveAt(current.Count - 1); // backtrack
        }
    }

    private HashSet<ProductItem> PrepareProductItemData(List<string> productItemCodes)
    {
        return productItemCodes.Select(code => new ProductItem
        {
            Code = code,
            Price = 0,
            ImageUrl = "",
            Quantity = 0
        }).ToHashSet();
    }
}
