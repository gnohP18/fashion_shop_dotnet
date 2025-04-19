using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.Infrastructure.Database;

namespace fashion_shop.Infrastructure.Repositories;

public class ProductVariantRepository : IProductVariantRepository
{
    public readonly ApplicationDbContext _dbContext;

    public ProductVariantRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IQueryable<ProductVariant> Queryable => _dbContext.ProductVariants.AsQueryable();
    public IUnitOfWork UnitOfWork => _dbContext;

    public Task AddAsync(ProductVariant entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Delete(ProductVariant entity)
    {
        throw new NotImplementedException();
    }

    public void DeleteByIds(IEnumerable<int> ids)
    {
        throw new NotImplementedException();
    }

    public void DeleteMany(List<ProductVariant> entities)
    {
        _dbContext.RemoveRange(entities);
    }

    public void Update(ProductVariant entity)
    {
        throw new NotImplementedException();
    }
}