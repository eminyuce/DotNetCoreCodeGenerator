using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DotNetCodeGenerator.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetCoreCodeGenerator.UnitTest
{

    [TestClass]
    public class UnitTest1
    {

        private static IConfigurationRoot GetConfig()
        {
            return new ConfigurationBuilder()
                        .SetBasePath(AppContext.BaseDirectory)
                        .AddJsonFile("appsettings.Development.json")
                        .Build();
        }

        //[TestMethod]
        //public void TestMethod21()
        //{
        //    IConfigurationRoot configuration = GetConfig();
        //    MySqlRepository pp = new MySqlRepository();
        //    //db_kodyazan
        //    var con = configuration.GetConnectionString("MyDefaultConnection");
        //    Console.WriteLine(con);
        //    var items = pp.GetNwmHaberlers(con);
        //    Console.WriteLine(items.Count);
        //}
        [TestMethod]
        public void TestMethod1()
        {
            IConfigurationRoot configuration = GetConfig();
            var serviceProvider = new ServiceCollection()
             .AddLogging()
             .BuildServiceProvider();

            var factory = serviceProvider.GetService<ILoggerFactory>();

            var logger = factory.CreateLogger<TableRepository>();


            var repo = new TableRepository(logger);

          Console.WriteLine(configuration.GetConnectionString("DefaultConnection"));
          repo.GetAllTables(configuration.GetConnectionString("DefaultConnection"));
         // repo.GetSelectedTableMetaData(configuration.GetConnectionString("DefaultConnection"), "TestEY.dbo.Products");
        }

    }
}
