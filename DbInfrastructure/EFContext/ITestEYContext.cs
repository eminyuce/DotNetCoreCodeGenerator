using EFGenericRepository;
using DbInfrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbInfrastructure.EFContext
{
    public interface ITestEYContext : IEntitiesContext
    {
        DbSet<Product> Products { get; set; }
    }
}
