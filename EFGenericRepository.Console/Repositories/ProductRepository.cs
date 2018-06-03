using EFGenericRepository.Console.EFContext;
using EFGenericRepository.Console.Repositories.IRepositories;
using EFGenericRepository;
using System;
using System.Collections.Generic;
using System.Text;
using EFGenericRepository.Console.Entities;

namespace EFGenericRepository.Console.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(ITestEYContext dbContext) : base(dbContext)
        {
        }
    }
}
