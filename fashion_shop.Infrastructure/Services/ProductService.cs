using AutoMapper;
using fashion_shop.Core.Common;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.DTOs.Responses;
using fashion_shop.Core.DTOs.Responses.Admin;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.Core.Interfaces.Services;
using fashion_shop.fashion_shop.Core.Exceptions;
using fashion_shop.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace fashion_shop.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public ProductService(
        IProductRepository productRepository,
        IMapper mapper,
        ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(_mapper));
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    }

    public async Task<CreateProductResponse> CreateAsync(CreateProductRequest request)
    {
        var product = _mapper.Map<Product>(request);

        var category = await _categoryRepository.GetByIdAsync(product.CategoryId);

        if (category is null)
        {
            throw new NotFoundException($"Not found category Id={product.CategoryId}");
        }

        product.Category = category;
        product.Slug = $"{request.Slug}-{DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")}";

        await _productRepository.AddAsync(product);
        await _productRepository.UnitOfWork.SaveChangesAsync();

        var dataMapping = _mapper.Map<CreateProductResponse>(product);

        return dataMapping;
    }

    public async Task<PaginationData<ProductDto>> GetListAsync(GetProductRequest request)
    {
        var query = _productRepository.GetAllQueryable();

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
            .Skip((request.Page - 1) * request.Limit)
            .Take(request.Limit)
            .ToListAsync();

        var dataMapping = _mapper.Map<IEnumerable<ProductDto>>(data);

        return new PaginationData<ProductDto>(dataMapping, request.Limit, request.Page, dataMapping.Count());
    }

    public async Task<ProductDto> GetDetailAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            throw new NotFoundException($"Not found product=Id {id}");
        }
        return _mapper.Map<ProductDto>(product);
    }

    public async Task DeleteAsync(int id)
    {
        await _productRepository.DeleteByIdAsync(id);
    }
}
