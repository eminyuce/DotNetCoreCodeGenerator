using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DbInfrastructure.Services.IServices
{
    public interface IBaseService<T> where T : class
    {
        List<T> LoadEntites(Expression<Func<T, bool>> whereLambda);
        T SaveOrUpdate(T entity, object key);
        T GetSingle(int id);
        List<T> GetAll();
        void DeleteEntity(T entity);


        Task<List<T>> LoadEntitesAsync(Expression<Func<T, bool>> whereLambda);
        Task<T> SaveOrUpdateAsync(T entity, object key);
        Task<T> GetSingleAsync(int id);
        Task<List<T>> GetAllAsync();
        Task<int> DeleteEntityAsync(T entity);

    }
}
