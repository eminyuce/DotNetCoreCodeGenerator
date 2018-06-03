using EFGenericRepository.Console.Repositories.IRepositories;
using EFGenericRepository.Console.Services.IServices;
using EFGenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using EFGenericRepository.Console.Entities;

namespace EFGenericRepository.Console.Services
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
