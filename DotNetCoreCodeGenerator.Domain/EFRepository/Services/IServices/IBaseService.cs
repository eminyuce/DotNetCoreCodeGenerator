using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace DotNetCoreCodeGenerator.Domain.EFRepository.Services.IServices
{
    public interface IBaseService<T> where T : class
    {
        List<T> LoadEntites(Expression<Func<T, bool>> whereLambda);
        T SaveOrEditEntity(T entity);
        T GetSingle(int id);
        List<T> GetAll();
        bool DeleteEntity(T entity);

    }
}
