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

    public IQueryable<OrderDetail> Queryable => _dbContext.OrderDetails.AsQueryable();

    public Task AddAsync(OrderDetail entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Delete(OrderDetail entity)
    {
        throw new NotImplementedException();
    }

    public void DeleteByIds(IEnumerable<int> ids)
    {
        throw new NotImplementedException();
    }

    public void DeleteMany(List<OrderDetail> entities)
    {
        throw new NotImplementedException();
    }

    public void Update(OrderDetail entity)
    {
        throw new NotImplementedException();
    }
}