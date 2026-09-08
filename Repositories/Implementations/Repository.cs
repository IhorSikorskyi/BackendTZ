using BackendTZ.Entities;
using BackendTZ.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackendTZ.Repositories.Implementations;

/// <summary>
/// Represents a generic repository for performing CRUD operations on entities of type T.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
/// <param name="dbContext">The database context.</param>
public class Repository<T>(DbContext dbContext) : IRepository<T> where T : BaseEntity
{
    /// <inheritdoc/>
    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.Set<T>().FindAsync([id], ct);
    }

    /// <inheritdoc/>
    public virtual async Task<bool> IsExistAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.Set<T>().AnyAsync(e => e.Id == id, ct); // типобезпечно, без reflection
    }


    /// <inheritdoc/>
    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
    {
        return await dbContext.Set<T>().AsNoTracking().ToListAsync(ct);
    }

    /// <inheritdoc/>
    public virtual async Task AddAsync(T entity, CancellationToken ct = default)
    {
        await dbContext.Set<T>().AddAsync(entity, ct);
    }

    /// <inheritdoc/>
    public virtual void Update(T entity)
    {
        dbContext.Set<T>().Update(entity);
    }

    /// <inheritdoc/>
    public virtual void Delete(T entity)
    {
        dbContext.Set<T>().Remove(entity);
    }
}