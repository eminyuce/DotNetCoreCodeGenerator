using DotNetCoreCodeGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCoreCodeGenerator.Domain.EFRepository.EFContext
{
    public interface ITestEYContext : IEntitiesContext
    {
        DbSet<Product> Products { get; set; }
    }
}
