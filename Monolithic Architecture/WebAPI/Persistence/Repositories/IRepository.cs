using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using WebAPI.Persistence.Dynamic;
using WebAPI.Persistence.Paging;

namespace WebAPI.Persistence.Repositories
{
    public interface IRepository<T> where T : Entity
    {
        T? Get(Expression<Func<T, bool>> filter);
        List<T> GetAll(Expression<Func<T, bool>> filter = null);
        T Add(T entity);
        ICollection<T> AddRange(ICollection<T> entities);
        T Delete(T entity);
        //ICollection<T> DeleteRange(ICollection<T> entity, bool permanent = false);
        T Update(T entity);
        ICollection<T> UpdateRange(ICollection<T> entities);

        bool Any(Expression<Func<T, bool>>? predicate = null, bool withDeleted = false, bool enableTracking = true);

        T? Get(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true
    );

        IPaginate<T> GetList(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            int index = 0,
            int size = 10,
            bool withDeleted = false,
            bool enableTracking = true
        );
        IPaginate<T> GetListByDynamic(
            DynamicQuery dynamic,
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            int index = 0,
            int size = 10,
            bool withDeleted = false,
            bool enableTracking = true
        );
    }
}