using DotNetCoreCodeGenerator.Domain.EFRepository.EFContext;
using DotNetCoreCodeGenerator.Domain.EFRepository.Repositories.IRepositories;
using DotNetCoreCodeGenerator.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCoreCodeGenerator.Domain.EFRepository.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(ITestEYContext dbContext) : base(dbContext)
        {
        }
    }
}
