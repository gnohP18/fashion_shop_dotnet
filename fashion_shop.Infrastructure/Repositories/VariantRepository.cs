using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.Infrastructure.Database;

namespace fashion_shop.Infrastructure.Repositories;

public class VariantRepository : IVariantRepository
{
    private readonly ApplicationDbContext _dbContext;

    public VariantRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IQueryable<Variant> Queryable => _dbContext.Variants.AsQueryable();

    public IUnitOfWork UnitOfWork => _dbContext;

    public Task AddAsync(Variant entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Delete(Variant entity)
    {
        throw new NotImplementedException();
    }

    public void DeleteByIds(IEnumerable<int> ids)
    {
        throw new NotImplementedException();
    }

    public void DeleteMany(List<Variant> entities)
    {
        _dbContext.Variants.RemoveRange(entities);
    }

    public void Update(Variant entity)
    {
        throw new NotImplementedException();
    }
}