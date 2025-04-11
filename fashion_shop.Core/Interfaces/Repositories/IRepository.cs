using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace fashion_shop.Core.Interfaces.Repositories;
/// <summary>
/// Generic repository interface for basic CRUD operations using async patterns.
/// </summary>
/// <typeparam name="T">Entity type.</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Get a single entity by its unique identifier.
    /// </summary>
    /// <param name="id">Entity ID.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The matching entity or null if not found.</returns>
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a single entity matching the given condition.
    /// </summary>
    /// <param name="predicate">A lambda expression to match the entity.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The matching entity or null if not found.</returns>
    Task<T?> GetOneAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get multiple entities by their IDs.
    /// </summary>
    /// <param name="ids">A collection of entity IDs.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A collection of matching entities.</returns>
    Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all entities of the given type.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A collection of all entities.</returns>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a new entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing entity asynchronously.
    /// </summary>
    /// <param name="entity">The updated entity.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete an entity by its ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the entity to delete.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    Task DeleteByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete multiple entities by their IDs asynchronously.
    /// </summary>
    /// <param name="ids">A collection of IDs to delete.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    Task DeleteByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);

    IQueryable<T> GetAllQueryable();

    IUnitOfWork UnitOfWork { get; }
}