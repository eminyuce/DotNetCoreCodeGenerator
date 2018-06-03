using DbInfrastructure.EFContext;
using DbInfrastructure.Entities;
using DbInfrastructure.Repositories;
using DbInfrastructure.Repositories.IRepositories;
using DbInfrastructure.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Infrastructure
{
    class Program
    {



        public static IConfigurationRoot Configuration { get; set; }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var builder = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("appsettings.json");

            var configuration = builder.Build();

            string MySqlDefaultConnection = configuration.GetConnectionString("MySqlDefaultConnection");
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            System.Console.WriteLine(connectionString);
            System.Console.WriteLine(MySqlDefaultConnection);
            ITestEYContext db = new TestEYContext(MySqlDefaultConnection);
            IProductRepository productRepository = new ProductRepository(db);
            var productService = new ProductService(productRepository);

            var item = new Product();
            item.Id = 1;
            item.StoreId = 1;
            item.ProductCategoryId = 1;
            item.BrandId = 1;
            item.RetailerId = 1;
            item.ProductCode = "";
            item.Name = "Name";
            item.Description = "";
            item.Type = "";
            item.MainPage = true;
            item.State = true;
            item.Ordering = 1;
            item.CreatedDate = DateTime.Now;
            item.ImageState = true;
            item.UpdatedDate = DateTime.Now;
            item.Price = 1000;
            productService.SaveOrEditEntity(item);

            var products = productService.GetAll();
            foreach (var p in products)
            {
                System.Console.WriteLine(p.Name);
            }


            System.Console.WriteLine("Press any key to continue...");
            System.Console.ReadLine();

        }
    }
}
