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

        public TestEYContext(string connectionString):base(connectionString)
        {
        }


    }
}
