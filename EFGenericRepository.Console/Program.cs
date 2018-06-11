using DbInfrastructure.EFContext;
using DbInfrastructure.Entities;
using DbInfrastructure.Repositories;
using DbInfrastructure.Repositories.IRepositories;
using DbInfrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Infrastructure
{
    class Program
    {



        public static IConfigurationRoot Configuration { get; set; }

        static void Main(string[] args)
        {
             NewMethod(); 
        }

        private static void NewMethod()
        {
            Console.WriteLine("Hello World!");
            var builder = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            var serviceProvider = new ServiceCollection()
             .AddLogging()
             .BuildServiceProvider();


            var factory = serviceProvider.GetService<ILoggerFactory>();


            string MySqlDefaultConnection = configuration.GetConnectionString("MySqlDefaultConnection");
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            System.Console.WriteLine(connectionString);
            System.Console.WriteLine(MySqlDefaultConnection);
            ITestEYContext db = new TestEYContext(MySqlDefaultConnection);

            //IProductRepository productRepository = new ProductRepository(db, factory.CreateLogger<ProductRepository>());
            var productService = new ProductService(new ProductRepository(db, factory), factory);

            for (int i = 0; i < 100; i++)
            {
                var item = new Product();
                item.StoreId = 1;
                item.ProductCategoryId = 1;
                item.BrandId = 1;
                item.RetailerId = 1;
                item.ProductCode = "";
                item.Name = "Name:"+Guid.NewGuid().ToString();
                item.Description = Guid.NewGuid().ToString();
                item.Type = "";
                item.MainPage = true;
                item.State = true;
                item.Ordering = 1;
                item.CreatedDate = DateTime.Now;
                item.ImageState = true;
                item.UpdatedDate = DateTime.Now;
                item.Price = 1000;
                productService.SaveOrUpdate(item, item.Id);

                Console.WriteLine(item.Name);
            }
          

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
