using System.Linq.Expressions;

namespace Reddit_App.Repository
{
    interface IRespositoryBase<T>
    {
        IQueryable<T> FindAll();
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);

        T Create(T entity);

        T UpdateByEntity(T entity);

        bool DeleteByEntity(T entity);
    }
}
