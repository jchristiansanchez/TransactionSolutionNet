using Company.App.Application.Interface.Persistence.Repositories.Base;
using Company.App.Domain;

namespace Company.App.Application.Interface.Persistence.Repositories
{
    public interface ITransactionRepository : IEFCoreBaseRepository<FinancialTransaction>
    {
        Task<FinancialTransaction> UpdateCurrentStateAsync(int id, string newState, CancellationToken cancellationToken);
        Task<FinancialTransaction?> FindByCorrelationIdAsync(Guid? correlationId, DateTime dateCreation);
    }
}
