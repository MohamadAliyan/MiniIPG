using System.Linq.Expressions;

namespace PaymentService.Domain.Common;

public interface IRepository<T> where T : class
{
    IQueryable<T> GetAll();
    void AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
    IQueryable<T> GetBy(Expression<Func<T, bool>> predicate);
    IQueryable<T> Get();
    Task<int> SaveChangesAsync();
}



