using DbInfrastructure.Repositories.IRepositories;
using DbInfrastructure.Services.IServices;
using EFGenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using DbInfrastructure.Entities;

namespace DbInfrastructure.Services
{
    public class ProductService : BaseService<Product>, IProductService
    {
        private IProductRepository ProductRepository { get; set; }
        public ProductService(IProductRepository baseRepository) : base(baseRepository)
        {
            ProductRepository = baseRepository;
          
        }

       
    }
}
