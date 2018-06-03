using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Threading.Tasks;

namespace EFGenericRepository
{
    public interface IEntitiesContext : IDisposable
    {

        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        void SetAsAdded<TEntity>(TEntity entity) where TEntity : class;
        void SetAsModified<TEntity>(TEntity entity) where TEntity : class;
        void SetAsDeleted<TEntity>(TEntity entity) where TEntity : class;
        int SaveChanges();
        Task<int> SaveChangesAsync();
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    }
}
