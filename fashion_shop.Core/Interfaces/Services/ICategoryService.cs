using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.DTOs.Responses;
using fashion_shop.Core.DTOs.Responses.Admin;


namespace fashion_shop.Core.Interfaces.Services;

public interface ICategoryService
{
    Task<CategoryDto?> GetDetailAsync(int id);
    Task<PaginationData<BasicCategoryDto>> GetListAsync(GetCategoryRequest request);
    Task<CreateCategoryResponse> CreateAsync(CreateCategoryRequest request);
    Task UpdateAsync(int id, UpdateCategoryRequest request);
    Task<bool> CheckExistSlugAsync(string slug);
    Task DeleteAsync(int id);
}