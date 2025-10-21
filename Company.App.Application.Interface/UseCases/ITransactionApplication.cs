using Company.App.Application.Dto;
using Company.App.Cross.Common;

namespace Company.App.Application.Interface.UseCases
{
    public interface ITransactionApplication
    {
        Task<ResponseT<int>> InsertTransactionAsync(TransactionDto transactionDto, CancellationToken cancellationToken = default);
        Task<ResponseT<int>> UpdateRejectedTransactionAsync(int id, Guid correlationId, Guid sourceAccountId, string message, CancellationToken cancellationToken = default);
        Task<ResponseT<int>> UpdateApprovedTransactionAsync(int id, Guid correlationId, Guid sourceAccountId, string message, CancellationToken cancellationToken = default);
        Task<ResponseT<TransactionResponseDto>> GetTransactionAsync(TransactionRequestDto request, CancellationToken cancellationToken = default);
    }
}