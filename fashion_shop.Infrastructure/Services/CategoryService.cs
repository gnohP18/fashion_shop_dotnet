using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.DTOs.Requests.Admin;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.Core.Interfaces.Services;

namespace fashion_shop.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task AddAsync(Category category)
    {
        await _categoryRepository.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
    }
}