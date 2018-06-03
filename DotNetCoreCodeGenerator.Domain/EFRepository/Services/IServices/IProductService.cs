using DotNetCoreCodeGenerator.Domain.EFRepository.Repositories.IRepositories;
using DotNetCoreCodeGenerator.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCoreCodeGenerator.Domain.EFRepository.Services.IServices
{
    public interface IProductService : IBaseService<Product>, IDisposable
    {

    }
   
}
