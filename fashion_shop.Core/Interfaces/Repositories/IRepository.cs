using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using fashion_shop.Core.DTOs.Requests.Admin;

namespace fashion_shop.Core.Interfaces.Repositories;
/// <summary>
/// Generic repository interface for basic CRUD operations using async patterns.
/// </summary>
/// <typeparam name="T">Entity type.</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Add a new entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing entity.
    /// </summary>
    /// <param name="entity">The updated entity.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    void Update(T entity);

    /// <summary>
    /// Delete an entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity to delete.</param>
    void Delete(T entity);

    /// <summary>
    /// Delete many entities by its ID.
    /// </summary>
    /// <param name="entities">List entities</param>
    void DeleteMany(List<T> entities);

    /// <summary>
    /// Delete multiple entities by their IDs.
    /// </summary>
    /// <param name="ids">A collection of IDs to delete.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    void DeleteByIds(IEnumerable<int> ids);

    /// <summary>
    /// Convert it to Query LINQ
    /// </summary>
    IQueryable<T> Queryable { get; }

    /// <summary>
    /// Apply Unit Of Work
    /// </summary>
    IUnitOfWork UnitOfWork { get; }
}