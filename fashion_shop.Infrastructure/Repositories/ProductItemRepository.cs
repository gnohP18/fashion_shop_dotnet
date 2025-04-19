using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.Infrastructure.Database;

namespace fashion_shop.Infrastructure.Repositories;

public class ProductItemRepository : IProductItemRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ProductItemRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IQueryable<ProductItem> Queryable => _dbContext.ProductItems.AsQueryable();

    public IUnitOfWork UnitOfWork => _dbContext;

    public Task AddAsync(ProductItem entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Delete(ProductItem entity)
    {
        throw new NotImplementedException();
    }

    public void DeleteByIds(IEnumerable<int> ids)
    {
        throw new NotImplementedException();
    }

    public void DeleteMany(List<ProductItem> entities)
    {
        _dbContext.ProductItems.RemoveRange(entities);
    }

    public void Update(ProductItem entity)
    {
        _dbContext.ProductItems.Update(entity);
    }

    public void UpdateManySelective(List<ProductItem> entities, params Expression<Func<ProductItem, object>>[] updatedProperties)
    {
        foreach (var entity in entities)
        {
            _dbContext.ProductItems.Attach(entity);

            foreach (var prop in updatedProperties)
            {
                _dbContext.Entry(entity).Property(prop).IsModified = true;
            }
        }
    }
}