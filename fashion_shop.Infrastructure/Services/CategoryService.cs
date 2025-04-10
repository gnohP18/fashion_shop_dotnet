using AutoMapper;
using fashion_shop.Core.Common;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.DTOs.Responses;
using fashion_shop.Core.DTOs.Responses.Admin;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.Core.Interfaces.Services;
using fashion_shop.fashion_shop.Core.Exceptions;
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
        var query = _categoryRepository.GetAllQueryable();

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
            .Skip((request.Page - 1) * request.Limit)
            .Take(request.Limit)
            .ToListAsync();

        var dataMapping = _mapper.Map<IEnumerable<CategoryDto>>(data);

        return new PaginationData<CategoryDto>(dataMapping, request.Limit, request.Page, total);
    }

    public async Task<CreateCategoryResponse> CreateAsync(CreateCategoryRequest request)
    {
        var category = _mapper.Map<Category>(request);

        if (await _categoryRepository.GetAllQueryable().AnyAsync(_ => _.Name.ToLower() == category.Name.ToLower()))
        {
            throw new BadRequestException("Name already existed");
        }

        await _categoryRepository.AddAsync(category);
        await _categoryRepository.UnitOfWork.SaveChangesAsync();

        return _mapper.Map<CreateCategoryResponse>(category);
    }

    public async Task DeleteAsync(int id)
    {
        if (await _productRepository.GetByIdAsync(id) is not null)
        {
            throw new BadRequestException("Can not delete category because it has products");
        }

        await _categoryRepository.DeleteByIdAsync(id);
    }
}