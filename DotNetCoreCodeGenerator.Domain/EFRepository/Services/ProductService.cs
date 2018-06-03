using DotNetCoreCodeGenerator.Domain.EFRepository.Repositories.IRepositories;
using DotNetCoreCodeGenerator.Domain.EFRepository.Services.IServices;
using DotNetCoreCodeGenerator.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DotNetCoreCodeGenerator.Domain.EFRepository.Services
{
    public class ProductService : BaseService<Product>, IProductService
    {
        private IProductRepository ProductRepository { get; set; }
        public ProductService(IProductRepository baseRepository) : base(baseRepository)
        {
            ProductRepository = baseRepository;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
