using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EFGenericRepository
{
    public abstract class EntitiesContext : DbContext, IEntitiesContext
    {

        /// <summary>
        /// Constructs a new context instance using conventions to create the name of
        /// the database to which a connection will be made. The by-convention name is
        /// the full name (namespace + class name) of the derived context class.  See
        /// the class remarks for how this is used to create a connection. 
        /// </summary>
        public EntitiesContext()
            : base()
        {
        }
        public EntitiesContext(DbContextOptions options) : base(options)
        {
        }
     


        /// <summary>
        /// Returns a DbSet instance for access to entities of the given type in the context.
        /// </summary>
        /// <remarks>
        /// This method calls the DbContext.Set method.
        /// </remarks>
        /// <typeparam name="TEntity">The type entity for which a set should be returned.</typeparam>
        /// <returns>A set for the given entity type.</returns>
        public new DbSet<TEntity> Set<TEntity>() where TEntity : class
        {

            return base.Set<TEntity>();
        }

        /// <summary>
        /// Sets the entity state as <see cref="EntityState.Added"/>.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity</typeparam>
        /// <param name="entity">The entity whose state needs to be set as <see cref="EntityState.Added"/>.</param>
        public void SetAsAdded<TEntity>(TEntity entity) where TEntity : class
        {

            EntityEntry dbEntityEntry = GetDbEntityEntrySafely(entity);
            dbEntityEntry.State = EntityState.Added;
        }

        /// <summary>
        /// Sets the entity state as <see cref="EntityState.Modified"/>.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity</typeparam>
        /// <param name="entity">The entity whose state needs to be set as <see cref="EntityState.Modified"/>.</param>
        public void SetAsModified<TEntity>(TEntity entity) where TEntity : class
        {

            EntityEntry dbEntityEntry = GetDbEntityEntrySafely(entity);
            dbEntityEntry.State = EntityState.Modified;
        }

        /// <summary>
        /// Sets the entity state as <see cref="EntityState.Deleted"/>.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity</typeparam>
        /// <param name="entity">The entity whose state needs to be set as <see cref="EntityState.Deleted"/>.</param>
        public void SetAsDeleted<TEntity>(TEntity entity) where TEntity : class
        {

            EntityEntry dbEntityEntry = GetDbEntityEntrySafely(entity);
            dbEntityEntry.State = EntityState.Deleted;
        }




        // privates
        private EntityEntry GetDbEntityEntrySafely<TEntity>(TEntity entity) where TEntity : class
        {

            EntityEntry dbEntityEntry = base.Entry<TEntity>(entity);
            if (dbEntityEntry.State == EntityState.Detached)
            {

                Set<TEntity>().Attach(entity);
            }

            return dbEntityEntry;
        }

        public async Task<int> SaveChangesAsync()
        {
            bool acceptAllChangesOnSuccess = true;
            CancellationToken cancellationToken = new CancellationToken();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
