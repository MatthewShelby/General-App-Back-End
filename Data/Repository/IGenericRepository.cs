using System;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors
{
    public interface IGenericRepository<TEntity> : IDisposable where TEntity : BaseEntity
    {
        IQueryable<TEntity> GetEntitiesQuery();

        Task<TEntity> GetEntityById(string entityId);

        Task AddEntity(TEntity entity);

        void UpdateEntity(TEntity entity);

        void RemoveEntity(TEntity entity);

        Task RemoveEntity(string entityId);

        Task SaveChanges();
    }
}