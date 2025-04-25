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
using fashion_shop.Infrastructure.Common;
using StackExchange.Redis;
using AutoMapper.QueryableExtensions;

namespace fashion_shop.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IDatabase _redis;


    public CategoryService(
        ICategoryRepository categoryRepository,
        IMapper mapper,
        IProductRepository productRepository,
        IConnectionMultiplexer connectionMultiplexer
    )
    {
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _redis = connectionMultiplexer.GetDatabase() ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
    }

    public async Task<PaginationData<BasicCategoryDto>> GetListAsync(GetCategoryRequest request)
    {
        var query = _categoryRepository
            .Queryable
            .AsNoTracking()
            .WithoutDeleted();

        if (!string.IsNullOrEmpty(request.KeySearch))
        {
            query = query.Where(
                x => EF.Functions.ILike(x.Name, $"%{request.KeySearch}%") ||
                    EF.Functions.ILike(x.Slug, request.KeySearch));
        }

        var total = await query.CountAsync();

        var sortByField = !string.IsNullOrEmpty(request.SortBy) ? request.SortBy : PaginationConstant.DefaultSortKey;

        query = request.Direction.ToUpper() == PaginationConstant.DefaultSortDirection
            ? query.OrderByDescending(x => request.SortBy)
            : query.OrderBy(x => request.SortBy);

        var data = await query
            .Select(c => new BasicCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                NumberOfProduct = c.Products.Count(p => p.IsDeleted == false)
            })
            .Skip((request.Page - 1) * request.Offset)
            .Take(request.Offset)
            .ToListAsync();

        data.ForEach(category =>
        {
            category.CanDelete = category.NumberOfProduct == 0;
        });

        return new PaginationData<BasicCategoryDto>(data, request.Offset, request.Page, total);
    }

    public async Task<CreateCategoryResponse> CreateAsync(CreateCategoryRequest request)
    {
        request.Name = request.Name.Trim();

        if (string.IsNullOrEmpty(request.Name))
        {
            throw new BadRequestException("Name is empty");
        }

        var category = _mapper.Map<Category>(request);

        if (await _categoryRepository.Queryable
            .WithoutDeleted()
            .AnyAsync(_ => _.Name.ToLower() == category.Name.ToLower()))
        {
            throw new BadRequestException("Name already existed");
        }

        await _categoryRepository.AddAsync(category);
        await _categoryRepository.UnitOfWork.SaveChangesAsync();

        return _mapper.Map<CreateCategoryResponse>(category);
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _categoryRepository
            .Queryable
            .WithoutDeleted()
            .FirstOrDefaultAsync(_ => _.Id == id);

        if (category is null)
        {
            throw new NotFoundException($"Not found category with Id={id}");
        }

        if (await _productRepository.Queryable.AnyAsync(_ => _.CategoryId == id))
        {
            throw new BadRequestException("Can not delete category because it has products");
        }

        _categoryRepository.Delete(category);
        await _categoryRepository.UnitOfWork.SaveChangesAsync();
    }

    public async Task<bool> CheckExistSlugAsync(string slug)
    {
        var slugHashed = Infrastructure.Common.Function.HashStringCRC32(slug);

        if (!await _redis.StringGetBitAsync(RedisConstant.CATEGORY_LIST, slugHashed))
        {
            return false;
        }

        return true;
    }

    public async Task UpdateAsync(int id, UpdateCategoryRequest request)
    {
        var category = await _categoryRepository.Queryable.FirstAsync(c => c.Id == id);

        category.Name = request.Name;
        category.Slug = request.Slug;

        _categoryRepository.Update(category);
        await _categoryRepository.UnitOfWork.SaveChangesAsync();
    }

    public async Task<CategoryDto?> GetDetailAsync(int id)
    {
        return await _categoryRepository
            .Queryable
            .ProjectTo<CategoryDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}