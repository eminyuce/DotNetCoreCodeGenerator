using EFGenericRepository.Console.EFContext;
using EFGenericRepository.Console.Repositories;
using EFGenericRepository.Console.Repositories.IRepositories;
using EFGenericRepository.Console.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace EFGenericRepository.Console
{
    class Program
    {



        public static IConfigurationRoot Configuration { get; set; }

        static void Main(string[] args)
        {
            // Console.WriteLine("Hello World!");
            var builder = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("appsettings.json");

            var configuration = builder.Build();

            string MySqlDefaultConnection = configuration.GetConnectionString("MySqlDefaultConnection");
            string connectionString = configuration.GetConnectionString("DefaultConnection");
           System.Console.WriteLine(connectionString);
           System.Console.WriteLine(MySqlDefaultConnection);
            TestEYContext db = new TestEYContext(connectionString);
            IProductRepository productRepository = new ProductRepository(db);
            var productService = new ProductService(productRepository);
            var products = productService.GetAll();
            foreach (var p in products)
            {
               System.Console.WriteLine(p.Name);
            }
            System.Console.ReadLine();
            System.Console.WriteLine("Press any key to continue...");
        }
    }
}
