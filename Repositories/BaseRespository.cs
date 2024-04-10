using Microsoft.EntityFrameworkCore;
using Reddit_App.Common;
using Reddit_App.Database;
using Reddit_App.Repositories;
using Reddit_App.Repository;
using System.Linq.Expressions;

namespace Reddit_App.Repositories
{
    public class BaseRespository<T> : IRespositoryBase<T> where T : class, new()
    {
        protected DbSet<T> Model { get; set; }

        protected DatabaseContext DbContext { get; set; }

        protected ApiOptions ApiOptions { get; set; }

        public BaseRespository(ApiOptions apiConfig,DatabaseContext databaseContext)
        {
            DbContext = databaseContext;
            Model = databaseContext.Set<T>();
            ApiOptions = apiConfig;
        }
        public T Create(T entity)
        {
            try
            {
                var model = Model.Add(entity);
                return model.Entity;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public bool DeleteByEntity(T entity)
        {
            try
            {
                Model.Remove(entity);
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return Model.Where(expression).AsNoTracking();
        }

        public async Task<int> CountByConditionAsync(Expression<Func<T, bool>> expression)
        {
            return await Model.CountAsync(expression);
        }

        public T UpdateByEntity(T entity)
        {
            try
            {
                Model.Update(entity);
                return entity;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        

        public void SaveChange()
        {
            DbContext.SaveChanges();
        }
        public async Task SaveChangeAsync()
        {
            await DbContext.SaveChangesAsync();
        }

        public IQueryable<T> FindAll()
        {
            return Model.AsNoTracking();
        }
    }
}
