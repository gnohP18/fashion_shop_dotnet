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

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _dbContext;

    public OrderRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IUnitOfWork UnitOfWork => _dbContext;

    public IQueryable<Order> Queryable => _dbContext.Orders.AsQueryable();

    public async Task AddAsync(Order entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.AddAsync(entity, cancellationToken);
    }

    public void Delete(Order entity)
    {
        throw new NotImplementedException();
    }

    public void DeleteByIds(IEnumerable<int> ids)
    {
        throw new NotImplementedException();
    }

    public void DeleteMany(List<Order> entities)
    {
        throw new NotImplementedException();
    }

    public void Update(Order entity)
    {
        throw new NotImplementedException();
    }
}