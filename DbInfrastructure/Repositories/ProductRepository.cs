using DbInfrastructure.EFContext;
using DbInfrastructure.Repositories.IRepositories;
using EFGenericRepository;
using System;
using System.Collections.Generic;
using System.Text;
using DbInfrastructure.Entities;
using Microsoft.Extensions.Logging;

namespace DbInfrastructure.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        private readonly ILogger<ProductRepository> Logger;

        public ProductRepository(ITestEYContext dbContext,
            ILoggerFactory loggerFactory) : base(dbContext)
        {
            Logger = loggerFactory.CreateLogger<ProductRepository>(); ;
        }
    }
}
