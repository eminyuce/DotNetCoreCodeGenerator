using DotNetCoreCodeGenerator.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCoreCodeGenerator.Domain.EFRepository.Repositories.IRepositories
{
    public interface IProductRepository : IBaseRepository<Product>, IDisposable
    {
    }
}
