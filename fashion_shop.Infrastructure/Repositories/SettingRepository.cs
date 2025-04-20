using fashion_shop.Core.Entities;
using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.Infrastructure.Database;

namespace fashion_shop.Infrastructure.Repositories;

public class SettingRepository : ISettingRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SettingRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IQueryable<Setting> Queryable => _dbContext.Settings.AsQueryable();

    public IUnitOfWork UnitOfWork => _dbContext;

    public Task AddAsync(Setting entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task AddManyAsync(List<Setting> entities)
    {
        await _dbContext.Settings.AddRangeAsync(entities);
    }

    public void Delete(Setting entity)
    {
        throw new NotImplementedException();
    }

    public void DeleteByIds(IEnumerable<int> ids)
    {
        throw new NotImplementedException();
    }

    public void DeleteMany(List<Setting> entities)
    {
        _dbContext.Settings.RemoveRange(entities);
    }

    public void Update(Setting entity)
    {
        throw new NotImplementedException();
    }
}