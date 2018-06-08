using EFGenericRepository;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DbInfrastructure.Repositories.IRepositories
{
    public interface IBaseRepository<T> : IEntityRepository<T, int> where T : class, IEntity<int>
    {
     
    }
}
