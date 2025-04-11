using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.fashion_shop.Core.Exceptions;
using fashion_shop.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace fashion_shop.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CategoryRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IUnitOfWork UnitOfWork => _dbContext;

    public async Task AddAsync(Category entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.AddAsync(entity, cancellationToken);
    }

    public async Task DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _dbContext.Categories.FirstOrDefaultAsync(_ => _.Id == id, cancellationToken);
        if (category != null)
        {
            _dbContext.Categories.Remove(category);
        }
        else
        {
            throw new NotFoundException("Not found product");
        }
    }

    public Task DeleteByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public IQueryable<Category> GetAllQueryable()
    {
        return _dbContext.Categories.AsNoTracking();
    }

    public async Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Categories.FirstOrDefaultAsync(_ => _.Id == id, cancellationToken);
    }

    public Task<IEnumerable<Category>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Category?> GetOneAsync(Expression<Func<Category, bool>> predicate, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Category entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}