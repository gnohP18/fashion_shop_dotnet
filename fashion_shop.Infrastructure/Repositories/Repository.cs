using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace fashion_shop.Infrastructure.Repositories;

/// <summary>
/// After implement UnitOfWork, We don't saveChange in Repository
/// </summary>
/// <typeparam name="T">Generic Class</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _dbContext;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<T>();
    }

    public IUnitOfWork UnitOfWork => _dbContext;

    public IQueryable<T> Queryable => _dbSet.AsQueryable();

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void DeleteByIds(IEnumerable<int> ids)
    {
        var entities = _dbSet.Where(e => ids.Contains(EF.Property<int>(e, "Id"))).ToList();
        _dbSet.RemoveRange(entities);
    }

    public void DeleteMany(List<T> entities)
    {
        throw new NotImplementedException();
    }
}