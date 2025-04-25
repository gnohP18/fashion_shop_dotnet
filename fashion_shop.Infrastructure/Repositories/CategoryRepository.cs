using fashion_shop.Core.Entities;
using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.Infrastructure.Database;

namespace fashion_shop.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CategoryRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IUnitOfWork UnitOfWork => _dbContext;

    public IQueryable<Category> Queryable => _dbContext.Categories.AsQueryable();

    public async Task AddAsync(Category entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.AddAsync(entity, cancellationToken);
    }

    public void Delete(Category entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Categories.Remove(entity);
    }

    public void Delete(Category entity)
    {
        _dbContext.Categories.Remove(entity);
    }

    public void DeleteByIds(IEnumerable<int> ids)
    {
        throw new NotImplementedException();
    }

    public void DeleteMany(List<Category> entities)
    {
        throw new NotImplementedException();
    }

    public void Update(Category entity)
    {
        _dbContext.Categories.Update(entity);
    }

    public Task UpdateAsync(Category entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}