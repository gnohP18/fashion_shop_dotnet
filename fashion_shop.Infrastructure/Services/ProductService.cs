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

namespace fashion_shop.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly MinioSettings _minioSettings;

    public ProductService(
        IProductRepository productRepository,
        IMapper mapper,
        IOptions<MinioSettings> options,
        ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(_mapper));
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        _minioSettings = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<CreateProductResponse> CreateAsync(CreateProductRequest request)
    {
        var product = _mapper.Map<Product>(request);

        var category = await _categoryRepository
            .Queryable
            .FirstOrDefaultAsync(_ => _.Id == product.CategoryId);

        if (category is null)
        {
            throw new NotFoundException($"Not found category Id={product.CategoryId}");
        }

        product.Category = category;
        product.Slug = Function.GenerateSlugProduct(request.Slug);

        await _productRepository.AddAsync(product);
        await _productRepository.UnitOfWork.SaveChangesAsync();

        var dataMapping = _mapper.Map<CreateProductResponse>(product);

        return dataMapping;
    }

    public async Task<PaginationData<ProductDto>> GetListAsync(GetProductRequest request)
    {
        var query = _productRepository.Queryable.AsNoTracking();

        if (!string.IsNullOrEmpty(request.KeySearch))
        {
            query = query.Where(
                x => x.Name.ToLower() == request.KeySearch.ToLower() ||
                x.Slug.ToLower() == request.KeySearch.ToLower());
        }

        var sortByField = !string.IsNullOrEmpty(request.SortBy) ? request.SortBy : PaginationConstant.DefaultSortKey;

        query = request.Direction.ToUpper() == PaginationConstant.DefaultSortDirection
            ? query.OrderByDescending(x => request.SortBy)
            : query.OrderBy(x => request.SortBy);

        query = query.Include(x => x.Category);

        var data = await query
            .Skip((request.Page - 1) * request.Offset)
            .Take(request.Offset)
            .Select(p => new ProductDto()
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

        return new PaginationData<ProductDto>(data, request.Offset, request.Page, total);
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
            }).FirstOrDefaultAsync(p => p.Id == id);

        return productDto ?? null;
    }

    public async Task DeleteAsync(int id)
    {
        var product = _productRepository.Queryable.FirstOrDefault(_ => _.Id == id);
        if (product is null)
        {
            throw new NotFoundException($"Not found product id={id}");
        }

        _productRepository.Delete(product);
        await _productRepository.UnitOfWork.SaveChangesAsync();
    }
}
