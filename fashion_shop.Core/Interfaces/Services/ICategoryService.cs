using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.DTOs.Responses;
using fashion_shop.Core.DTOs.Responses.Admin;


namespace fashion_shop.Core.Interfaces.Services;

public interface ICategoryService
{
    Task<PaginationData<CategoryDto>> GetListAsync(GetCategoryRequest request);
    Task<CreateCategoryResponse> CreateAsync(CreateCategoryRequest request);
    Task DeleteAsync(int id);
}