using Company.App.Application.Interface.Persistence.Repositories;
using Company.App.Domain;
using Company.App.Persistence.Context.EFCore;
using Company.App.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Company.App.Persistence.Repositories
{
    public class TransactionRepository : EFCoreBaseRepository<FinancialTransaction>, ITransactionRepository
    {
        private readonly EFCoreContext _applicationDbContext;
        public TransactionRepository(EFCoreContext applicationDbContext) : base(applicationDbContext)   
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<FinancialTransaction?> FindByCorrelationIdAsync(Guid? correlationId, DateTime dateCreation)
        {
            return await _applicationDbContext.FinancialTransaction.FirstOrDefaultAsync(t =>t.CorrelationId == correlationId && t.DateCreation.Date == dateCreation.Date);
        }

        public async Task<FinancialTransaction> UpdateCurrentStateAsync(int id, string newState, CancellationToken cancellationToken)
        {
            var entity = new FinancialTransaction { Id = id, CurrentState = newState };

            _applicationDbContext.Attach(entity);
            _applicationDbContext.Entry(entity).Property(e => e.CurrentState).IsModified = true;

            await _applicationDbContext.SaveChangesAsync(cancellationToken);

            return entity;
        }
    }
}
