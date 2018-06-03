using EFGenericRepository;
using EFGenericRepository.Console.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFGenericRepository.Console.EFContext
{
    public interface ITestEYContext : IEntitiesContext
    {
        DbSet<Product> Products { get; set; }
    }
}
