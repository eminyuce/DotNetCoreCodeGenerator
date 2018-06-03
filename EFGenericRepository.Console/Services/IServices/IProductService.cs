using EFGenericRepository.Console.Repositories.IRepositories;
using EFGenericRepository;
using System;
using System.Collections.Generic;
using System.Text;
using EFGenericRepository.Console.Entities;

namespace EFGenericRepository.Console.Services.IServices
{
    public interface IProductService : IBaseService<Product>, IDisposable
    {

    }
   
}
