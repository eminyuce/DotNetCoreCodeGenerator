using EFGenericRepository.Console.Entities;
using EFGenericRepository.Console.Repositories;
using EFGenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFGenericRepository.Console.EFContext
{
    public class TestEYContext : EntitiesContext, ITestEYContext
    {
        public DbSet<Product> Products { get; set; }
        private readonly string _connectionString;

        public TestEYContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseMySql(_connectionString);
        }

        public TestEYContext():base()
        {
        }


    }
}
