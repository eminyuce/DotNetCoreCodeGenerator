using DbInfrastructure.Repositories.IRepositories;
using EFGenericRepository;
using System;
using System.Collections.Generic;
using System.Text;
using DbInfrastructure.Entities;

namespace DbInfrastructure.Services.IServices
{
    public interface IProductService : IBaseService<Product>, IDisposable
    {

    }
   
}
