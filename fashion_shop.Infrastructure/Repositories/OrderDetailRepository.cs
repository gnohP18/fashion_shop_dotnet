using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace fashion_shop.Infrastructure.Repositories;

public class OrderDetailRepository : IOrderDetailRepository
{
    private readonly ApplicationDbContext _dbContext;
    public OrderDetailRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IUnitOfWork UnitOfWork => _dbContext;

    public Task AddAsync(OrderDetail entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<OrderDetail>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public IQueryable<OrderDetail> GetAllQueryable()
    {
        return _dbContext.OrderDetails.AsNoTracking();
    }

    public Task<OrderDetail?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<OrderDetail>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<OrderDetail?> GetOneAsync(Expression<Func<OrderDetail, bool>> predicate, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(OrderDetail entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}