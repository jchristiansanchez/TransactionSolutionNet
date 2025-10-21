using Company.App.Application.Interface.Persistence.Repositories.Base;
using Company.App.Domain.Common;
using Company.App.Persistence.Context.EFCore;
using Microsoft.EntityFrameworkCore;

namespace Company.App.Persistence.Repositories.Base
{
    public class EFCoreBaseRepository<TEntity> : IEFCoreBaseRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly EFCoreContext _context;
        public EFCoreBaseRepository(EFCoreContext context)
        {
            _context = context;
        }
        public async Task<TEntity> AddEntityAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            _context.Set<TEntity>().Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task DeleteEntityAsync(TEntity entity, CancellationToken cancellationToken)
        {
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
          
        public async Task<IReadOnlyList<TEntity>> GetAllEntityAsync(CancellationToken cancellationToken)
        {
            return await _context.Set<TEntity>().ToListAsync(cancellationToken);
        }
       
        public async Task<TEntity> GetEntityByIdAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id, cancellationToken);

            return entity;
        }
        public async Task<TEntity> UpdateEntityAsync(TEntity entity, CancellationToken cancellationToken)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }


        #region NoAsync
        public void DeleteEntityVoid(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }
        public void AddEntityVoid(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
        }
        public void UpdateEntityVoid(TEntity entity)
        {
            _context.Set<TEntity>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
        #endregion

    }
}
