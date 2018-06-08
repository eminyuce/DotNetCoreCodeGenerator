using EFGenericRepository;
using DbInfrastructure.EFContext;
using DbInfrastructure.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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

 
     

     
 
    }
}
