using System;
using System.Linq;
using System.Threading.Tasks;
using Doctors.Data;
using Microsoft.EntityFrameworkCore;

namespace Doctors
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        #region constructor

        private ApplicationDbContext context;
        private DbSet<TEntity> dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            this.context = context;
            this.dbSet = this.context.Set<TEntity>();
        }

        #endregion


        public IQueryable<TEntity> GetEntitiesQuery()
        {
            return dbSet.AsQueryable();
        }

        public async Task<TEntity> GetEntityById(string entityId)
        {
            return await dbSet.SingleOrDefaultAsync(e => e.Id == entityId);
        }

        public async Task AddEntity(TEntity entity)
        {
            entity.CreateDate = DateTime.Now;
            entity.LastUpdateDate = entity.CreateDate;
            await dbSet.AddAsync(entity);
        }

        public void UpdateEntity(TEntity entity)
        {
            entity.LastUpdateDate = DateTime.Now;
            dbSet.Update(entity);
        }

        public void RemoveEntity(TEntity entity)
        {
            entity.IsDelete = true;
            UpdateEntity(entity);
        }

        public async Task RemoveEntity(string entityId)
        {
            var entity = await GetEntityById(entityId);
            RemoveEntity(entity);
        }

        public async Task SaveChanges()
        {
            await context.SaveChangesAsync();
        }

        public void Dispose()
        {
            context?.Dispose();
        }
    }
}
