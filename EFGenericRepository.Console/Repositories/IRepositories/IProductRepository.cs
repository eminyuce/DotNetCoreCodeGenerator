using EFGenericRepository.Console.Entities;
using EFGenericRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFGenericRepository.Console.Repositories.IRepositories
{
    public interface IProductRepository : IBaseRepository<Product>, IDisposable
    {
    }
}
