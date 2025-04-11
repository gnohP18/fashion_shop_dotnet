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

    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(id, cancellationToken);
    }

    public async Task<T?> GetOneAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(e => ids.Contains(EF.Property<int>(e, "Id"))).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FindAsync(id, cancellationToken);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public async Task DeleteByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        var entities = await _dbSet.Where(e => ids.Contains(EF.Property<int>(e, "Id"))).ToListAsync(cancellationToken);
        _dbSet.RemoveRange(entities);
    }

    public IQueryable<T> GetAllQueryable()
    {
        return _dbSet.AsNoTracking();
    }
}