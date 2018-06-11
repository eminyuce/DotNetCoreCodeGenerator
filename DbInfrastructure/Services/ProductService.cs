using DbInfrastructure.Repositories.IRepositories;
using DbInfrastructure.Services.IServices;
using EFGenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using DbInfrastructure.Entities;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DbInfrastructure.Services
{
    public class ProductService : BaseService<Product>, IProductService
    {
        private IProductRepository ProductRepository { get; set; }
        private ILogger<ProductService> Logger { get; set; }


        public ProductService(IProductRepository baseRepository,
            ILoggerFactory loggerFactory) : base(baseRepository)
        {
            ProductRepository = baseRepository;
            Logger = loggerFactory.CreateLogger<ProductService>();
        }
        public async Task<IEnumerable<SelectListItem>> GetProductListItems()
        {
            Logger.LogInformation("GetTypes called.");
            var products = await ProductRepository.GetAllAsync();
            var items = new List<SelectListItem>
            {
                new SelectListItem() { Value = null, Text = "All", Selected = true }
            };
            foreach (Product product in products)
            {
                items.Add(new SelectListItem() { Value = product.Id.ToString(),
                    Text = product.Name });
            }

            return items;
        }

    }
}
