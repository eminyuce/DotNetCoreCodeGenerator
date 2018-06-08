using EFGenericRepository;
using DbInfrastructure.EFContext;
using DbInfrastructure.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace DbInfrastructure.Repositories
{
    public abstract class BaseRepository<T> : EntityRepository<T, int>, IBaseRepository<T> 
      where T : class, IEntity<int>
    {

        public ITestEYContext DbContext;

        public BaseRepository(ITestEYContext dbContext) : base(dbContext)
        {
            DbContext = dbContext;
            //((EImeceContext)DbContext).Configuration.LazyLoadingEnabled = false;
            //((EImeceContext)DbContext).Configuration.ProxyCreationEnabled = false;
            //    EImeceDbContext.Database.Log = s => BaseLogger.Trace(s);

        }

     
     
       
        public List<Expression<Func<T, object>>> GetIncludePropertyExpressionList()
        {
            return new List<Expression<Func<T, object>>>();
        }

        public int SaveOrEdit(T item)
        {
            if ((int)item.Id == 0)
            {
                this.Add(item);
            }
            else
            {
                this.Edit(item);
            }

            return this.Save();
        }

        public int DeleteItem(T item)
        {
            this.Delete(item);
            return this.Save();
        }

        public bool DeleteByWhereCondition(Expression<Func<T, bool>> whereLambda)
        {
            this.Delete(whereLambda);
            return this.Save() == 1;
        }

     
    }
}
