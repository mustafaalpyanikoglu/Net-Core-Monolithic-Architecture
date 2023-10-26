using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using WebAPI.Persistence.Dynamic;
using WebAPI.Persistence.Paging;

namespace WebAPI.Persistence.Repositories;

public interface IAsyncRepository<T> : IQuery<T> where T : Entity
{
    Task<T?> GetAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>,
                  IIncludableQueryable<T, object>>? include = null, bool enableTracking = true,
                  CancellationToken cancellationToken = default);

    Task<IPaginate<T>> GetListAsync(Expression<Func<T, bool>>? predicate = null,
                                Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
                                int index = 0, int size = 1000, bool enableTracking = true,
                                CancellationToken cancellationToken = default);

    Task<IPaginate<T>> GetListByDynamicAsync(DynamicQuery dynamic,
                                         Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
                                         int index = 0, int size = 10, bool enableTracking = true,
                                         CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(
    Expression<Func<T, bool>>? predicate = null,
    bool withDeleted = false,
    bool enableTracking = true,
    CancellationToken cancellationToken = default
);
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<T> DeleteAsync(T entity);
    Task<ICollection<T>> AddRangeAsync(ICollection<T> entity);
    //Task<ICollection<T>> DeleteRangeAsync(ICollection<T> entity, bool permanent = false);
    Task<ICollection<T>> UpdateRangeAsync(ICollection<T> entity);
}