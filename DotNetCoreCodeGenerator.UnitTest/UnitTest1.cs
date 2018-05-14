using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DotNetCodeGenerator.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using DotNetCodeGenerator.Domain.Helpers;
using System.Diagnostics;
using DotNetCodeGenerator.Domain.Entities;

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
        public void ParseSqlStatement()
        {
            string txt = @"

USE [TestEY]
GO

/****** Object:  Table [dbo].[Products]    Script Date: 4/28/2018 12:53:17 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Products](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StoreId] [int] NULL,
	[ProductCategoryId] [int] NOT NULL,
	[BrandId] [int] NULL,
	[RetailerId] [int] NULL,
	[ProductCode] [nvarchar](50) NULL,
	[Name] [nvarchar](500) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Type] [nvarchar](50) NULL,
	[MainPage] [bit] NULL,
	[State] [bit] NULL,
	[Ordering] [int] NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ImageState] [bit] NULL,
	[UpdatedDate] [datetime2](7) NOT NULL,
	[Price] [float] NOT NULL,
	[Discount] [float] NOT NULL,
	[UnitsInStock] [int] NULL,
	[TotalRating] [int] NULL,
	[VideoUrl] [nvarchar](1500) NULL,
 CONSTRAINT [PK_Products_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Products] ADD  CONSTRAINT [DF_Products_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO

ALTER TABLE [dbo].[Products] ADD  CONSTRAINT [DF_Products_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
GO



";


            String mySql = @"
CREATE TABLE `urunler` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `baslik_tr` varchar(255) DEFAULT NULL,
  `keywords_tr` varchar(255) DEFAULT NULL,
  `katID` varchar(11) DEFAULT '0',
  `ozet_tr` text,
  `detay_tr` text,
  `sira` int(11) DEFAULT '1000',
  `tarih` datetime DEFAULT NULL,
  `durum` tinyint(1) DEFAULT NULL,
  `baslik_en` varchar(255) DEFAULT NULL,
  `keywords_en` varchar(255) DEFAULT NULL,
  `ozet_en` text,
  `detay_en` text,
  `seo` varchar(255) DEFAULT NULL,
  `tip` int(4) DEFAULT '1' COMMENT '1: motor, 2: yelken',
  `link` varchar(255) DEFAULT NULL,
  `vitrin` int(1) DEFAULT '0',
  `image` varchar(255) DEFAULT NULL,
  `youtube` varchar(255) DEFAULT NULL,
  `fiyat` double DEFAULT NULL,
  `tamam` varchar(255) CHARACTER SET utf8 COLLATE utf8_turkish_ci DEFAULT NULL,
  `baslik_de` varchar(255) DEFAULT NULL,
  `keywords_de` varchar(255) DEFAULT NULL,
  `ozet_de` text,
  `detay_de` text,
  `adres_de` text,
  `online` tinyint(1) DEFAULT NULL,
  `ColorID` int(11) DEFAULT NULL,
  `RegionID` int(11) DEFAULT NULL,
  `GrapeID` int(11) DEFAULT NULL,
  `yemek_tercihi_tr` text,
  `haftanin` tinyint(1) DEFAULT NULL,
  `yemek_tercihi_en` text,
  `stok` varchar(255) DEFAULT NULL,
  `yeni` tinyint(1) DEFAULT NULL,
  `yil` varchar(255) DEFAULT NULL,
  `miktar` varchar(255) DEFAULT NULL,
  `eski_fiyat` varchar(255) DEFAULT NULL,
  `alkol_orani` varchar(255) DEFAULT NULL,
  `kdv` varchar(255) DEFAULT NULL,
  `encok` tinyint(1) DEFAULT NULL,
  `harita` text CHARACTER SET utf8 COLLATE utf8_turkish_ci NOT NULL,
  `baslik_ar` varchar(255) DEFAULT NULL,
  `keywords_ar` varchar(255) DEFAULT NULL,
  `ozet_ar` text,
  `detay_ar` text,
  `baslik_ru` varchar(255) DEFAULT NULL,
  `keywords_ru` varchar(255) DEFAULT NULL,
  `ozet_ru` text,
  `detay_ru` text,
  `teknik` text,
  `renk` varchar(255) DEFAULT NULL,
  `kilit` varchar(255) DEFAULT NULL,
  `kaplama` varchar(255) DEFAULT NULL,
  `aksesuar` varchar(255) DEFAULT NULL,
  `aksesuarr` varchar(255) DEFAULT NULL,
  `markaID` int(11) DEFAULT NULL,
  `sektorID` int(11) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=13 DEFAULT CHARSET=utf8;

";

            mySql = @"CREATE TABLE Persons (
    PersonID int,
    LastName varchar(255),
    FirstName varchar(255),
    Address varchar(255),
    City varchar(255) 
); ";



            // Create new stopwatch.
            Stopwatch stopwatch = new Stopwatch();

            // Begin timing.
            stopwatch.Start();


            IConfigurationRoot configuration = GetConfig();
            var serviceProvider = new ServiceCollection()
             .AddLogging()
             .BuildServiceProvider();


            var factory = serviceProvider.GetService<ILoggerFactory>();

            var logger = factory.CreateLogger<TableRepository>();

            var r = new CodeGeneratorResult();
            r.ModifiedTableName = "NwmProducts";

            var parser = new SqlParserHelper(factory.CreateLogger<SqlParserHelper>());
            var metadata = parser.ParseSqlCreateStatement(txt);
            // Stop timing.
            stopwatch.Stop();

            // Write result.
            Console.WriteLine("Time elapsed: {0}", stopwatch.ElapsedMilliseconds);


            CodeProducerHelper CodeProducerHelper = new CodeProducerHelper(factory.CreateLogger<CodeProducerHelper>());
            CodeProducerHelper.DatabaseMetadata = metadata;
            CodeProducerHelper.CodeGeneratorResult = r;
            CodeProducerHelper.GenerateTableItem();
            CodeProducerHelper.GenerateSaveOrUpdateStoredProcedure();
            CodeProducerHelper.GenerateMergeSqlStoredProcedure();
            Console.WriteLine(r.MergeSqlStoredProcedure);
        }

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
            var databaseMetaData = repo.GetAllTables(configuration.GetConnectionString("DefaultConnection"));
            repo.GetSelectedTableMetaData(databaseMetaData, "TestEY.dbo.TestTable-TestTable");
            var p = new CodeProducerHelper(factory.CreateLogger<CodeProducerHelper>());
            p.DatabaseMetadata = databaseMetaData;
            p.CodeGeneratorResult = new DotNetCodeGenerator.Domain.Entities.CodeGeneratorResult();
            p.GenerateMergeSqlStoredProcedure();
            Console.WriteLine(p.CodeGeneratorResult.MergeSqlStoredProcedure);

        }

    }
}
