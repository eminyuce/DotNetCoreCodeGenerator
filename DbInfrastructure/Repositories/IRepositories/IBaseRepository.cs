using EFGenericRepository;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace DbInfrastructure.Repositories.IRepositories
{
    public interface IBaseRepository<T> : IDisposable , IEntityRepository<T, int> where T : class, IEntity<int>
    {
        int SaveOrEdit(T item);
        int DeleteItem(T item);
        bool DeleteByWhereCondition(Expression<Func<T, bool>> whereLambda);
    }
}
