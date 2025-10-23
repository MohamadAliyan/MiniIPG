using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;
using GatewayService.Domain;
using GatewayService.Domain.Common;
using GatewayService.Infrastructure.Persistence;

namespace GatewayService.Infrastructure.Common;

public class BaseRepository<T> : IRepository<T> where T : class
{
    private readonly ApplicationContext _context;
    private readonly DbSet<T> _entity;
    private string errorMessage = string.Empty;

    public BaseRepository(ApplicationContext context)
    {
        _context = context;

        _entity = context.Set<T>();
    }

    public virtual IQueryable<T> GetAll()
    {
        return _entity;
    }

    public void AddAsync(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }


        _entity.AddAsync(entity);
    }

    public void Update(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }
    }

    public void Remove(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }


        _entity.Remove(entity);
    }

    public virtual IQueryable<T> GetBy(Expression<Func<T, bool>> predicate)
    {
        return _entity.Where(predicate);
    }

    public IQueryable<T> Get()
    {
        return _entity.AsQueryable();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
