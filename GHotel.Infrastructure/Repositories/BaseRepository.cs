using System.Linq.Expressions;
using GHotel.Domain.Entities;
using GHotel.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GHotel.Infrastructure.Repositories;

public class BaseRepository<T> where T : Entity
{
    protected readonly DbContext _dbContext;
    protected readonly DbSet<T> _dbSet;

    protected BaseRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAll(CancellationToken cancellationToken)
    {
        return await _dbSet.ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<T?> Get(CancellationToken cancellationToken, params object[] key)
    {
        var targetEntity = await _dbSet.FindAsync(key, cancellationToken).ConfigureAwait(false);
        return targetEntity;
    }

    public async Task<T?> Get(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
    }

    public async Task<T> Add(T entity, CancellationToken cancellationToken)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));
        var addedEntitiy = await _dbSet.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        return addedEntitiy.Entity;
    }

    public async Task<T?> Update(T entity, CancellationToken cancellationToken)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));
        var targetEntity = await _dbSet.AsNoTracking().FirstOrDefaultAsync(en => en.Id == entity.Id, cancellationToken).ConfigureAwait(false);
        if (targetEntity == null)
            return null;
        entity.CreatedAtUtc = targetEntity.CreatedAtUtc;
        var updatedEntity = _dbContext.Update(entity);
        return updatedEntity?.Entity;
    }

    public T Remove(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        entity.EntitiyStatus = EntityStatus.Deleted;
        var updatedEntity = _dbContext.Update(entity);
        return updatedEntity.Entity;
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));
        foreach (var entity in entities)
        {
            entity.EntitiyStatus = EntityStatus.Deleted;
            _dbContext.Update(entity);
        }
    }

    public async Task<T?> Remove(CancellationToken cancellationToken, params object[] key)
    {
        var entity = await _dbSet.FindAsync(key).ConfigureAwait(false);
        if (entity == null)
            return null;
        entity.EntitiyStatus = EntityStatus.Deleted;
        _dbContext.Update(entity);

        return entity;
    }

    public async Task<bool> Any(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IEnumerable<T>> GetAll(Expression<Func<T,bool>> predicate,CancellationToken cancellationToken)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public IQueryable<T> GetAllAsQueryable(Expression<Func<T,bool>> predicate,CancellationToken cancellationToken)
    {
        return _dbSet.Where(predicate);
    }

}
