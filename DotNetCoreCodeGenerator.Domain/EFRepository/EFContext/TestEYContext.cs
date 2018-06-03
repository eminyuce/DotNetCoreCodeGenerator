using DotNetCoreCodeGenerator.Domain.EFRepository.Repositories;
using DotNetCoreCodeGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCoreCodeGenerator.Domain.EFRepository.EFContext
{
    public class TestEYContext : EntitiesContext, ITestEYContext
    {
        public DbSet<Product> Products { get; set; }

        public TestEYContext(string connectionString):base(connectionString)
        {
        }


    }
}
