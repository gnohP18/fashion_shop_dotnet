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

namespace fashion_shop.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository categoryRepository, IMapper mapper, IProductRepository productRepository)
    {
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    }

    public async Task<PaginationData<CategoryDto>> GetListAsync(GetCategoryRequest request)
    {
        var query = _categoryRepository.Queryable.AsNoTracking();

        var total = await query.CountAsync();

        if (!string.IsNullOrEmpty(request.Name))
        {
            query = query.Where(x => x.Name.ToLower() == request.Name.ToLower());
        }

        var sortByField = !string.IsNullOrEmpty(request.SortBy) ? request.SortBy : PaginationConstant.DefaultSortKey;

        query = request.Direction.ToUpper() == PaginationConstant.DefaultSortDirection
            ? query.OrderByDescending(x => request.SortBy)
            : query.OrderBy(x => request.SortBy);

        var data = await query
            .Skip((request.Page - 1) * request.Offset)
            .Take(request.Offset)
            .ToListAsync();

        var dataMapping = _mapper.Map<IEnumerable<CategoryDto>>(data);

        return new PaginationData<CategoryDto>(dataMapping, request.Offset, request.Page, total);
    }

    public async Task<CreateCategoryResponse> CreateAsync(CreateCategoryRequest request)
    {
        request.Name = request.Name.Trim();

        if (string.IsNullOrEmpty(request.Name))
        {
            throw new BadRequestException("Name is empty");
        }

        var category = _mapper.Map<Category>(request);

        if (await _categoryRepository.Queryable.AnyAsync(_ => _.Name.ToLower() == category.Name.ToLower()))
        {
            throw new BadRequestException("Name already existed");
        }

        await _categoryRepository.AddAsync(category);
        await _categoryRepository.UnitOfWork.SaveChangesAsync();

        return _mapper.Map<CreateCategoryResponse>(category);
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _categoryRepository.Queryable.FirstOrDefaultAsync(_ => _.Id == id);

        if (category is null)
        {
            throw new NotFoundException($"Not found category with Id={id}");
        }

        if (await _productRepository.Queryable.FirstOrDefaultAsync(_ => _.Id == id) is not null)
        {
            throw new BadRequestException("Can not delete category because it has products");
        }

        _categoryRepository.Delete(category);
        await _categoryRepository.UnitOfWork.SaveChangesAsync();
    }
}