using DbInfrastructure.EFContext;
using DbInfrastructure.Repositories.IRepositories;
using EFGenericRepository;
using System;
using System.Collections.Generic;
using System.Text;
using DbInfrastructure.Entities;

namespace DbInfrastructure.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(ITestEYContext dbContext) : base(dbContext)
        {
         
        }
    }
}
