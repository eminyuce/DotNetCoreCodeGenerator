using DbInfrastructure.Entities;
using EFGenericRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbInfrastructure.Repositories.IRepositories
{
    public interface IProductRepository : IBaseRepository<Product>
    {
    }
}
