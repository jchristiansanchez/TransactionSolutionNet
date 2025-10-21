using Company.App.Domain.Common;

namespace Company.App.Application.Interface.Persistence.Repositories.Base
{
    public interface IEFCoreBaseRepository<TEntity> where TEntity : BaseEntity
    {
        Task<IReadOnlyList<TEntity>> GetAllEntityAsync(CancellationToken cancellationToken = default);
        Task<TEntity> GetEntityByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<TEntity> AddEntityAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<TEntity> UpdateEntityAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task DeleteEntityAsync(TEntity entity, CancellationToken cancellationToken = default);
        void AddEntityVoid(TEntity entity);
        void UpdateEntityVoid(TEntity entity);
        void DeleteEntityVoid(TEntity entity);
    }
}
