using Company.App.Application.Dto;
using Company.App.Application.Interface.Infrastructure.ConfluentKafka;
using Company.App.Application.Interface.Persistence.Repositories;
using Company.App.Application.Interface.UseCases;
using Company.App.Cross.Common;
using Company.App.Domain;
using Company.App.Domain.Constants;
using Company.App.Domain.Events.ConfluentKafka;
using FluentValidation;

namespace Company.App.Application.UseCases.Transaction
{
    public class TransactionApplication : ITransactionApplication
    {
        private readonly IKafkaProducer _eventBus;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IValidator<TransactionDto> _validator;
        private readonly IValidator<TransactionRequestDto> _validatorRequest;   
        public TransactionApplication(ITransactionRepository transactionRepository, IKafkaProducer eventBus, IValidator<TransactionDto> validator, IValidator<TransactionRequestDto> validatorRequest)
        {
            _transactionRepository = transactionRepository;
            _eventBus = eventBus;
            _validator = validator;
            _validatorRequest = validatorRequest;
        }

        public async Task<ResponseT<TransactionResponseDto>> GetTransactionAsync(TransactionRequestDto request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validatorRequest.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ResponseT<TransactionResponseDto>
                {
                    IsSuccess = false,
                    Message = "Validation errors",
                    ListErrors = [.. validationResult.Errors.Select(e => e.ErrorMessage)]
                };
            }

            var transaction = await _transactionRepository.FindByCorrelationIdAsync(request?.TransactionExternalId, request.CreatedAt);

            if (transaction == null)
            {
                return new ResponseT<TransactionResponseDto>
                {
                    IsSuccess = false,
                    Message = "Transaction not found"
                };
            }

            return new ResponseT<TransactionResponseDto>
            {
                IsSuccess = true,
                Data = new TransactionResponseDto
                {
                    TransactionExternalId = transaction.CorrelationId,
                    SourceAccountId = transaction.SourceAccountId,
                    TargetAccountId = transaction.TargetAccountId,
                    TranferTypeId = transaction.TranferTypeId,
                    Value = transaction.Value,
                    CurrentState = transaction.CurrentState
                }
            };

        }

        public async Task<ResponseT<int>> InsertTransactionAsync(TransactionDto transactionDto, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validator.ValidateAsync(transactionDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ResponseT<int>
                {
                    IsSuccess = false,
                    Message = "Validation errors",
                    ListErrors = [.. validationResult.Errors.Select(e => e.ErrorMessage)]
                };
            }

            var entityTransaction = new FinancialTransaction
            {
                CorrelationId = Guid.NewGuid(),
                SourceAccountId = transactionDto?.SourceAccountId,
                TargetAccountId = transactionDto?.TargetAccountId,
                TranferTypeId = transactionDto.TranferTypeId,
                CurrentState = CurrentStateConstants.PENDING,
                Value = transactionDto.Value
            };

            var entity = await _transactionRepository.AddEntityAsync(entityTransaction, cancellationToken);

            if (entity == null)
            {
                return new ResponseT<int>
                {
                    IsSuccess = false,
                    Message = "Failed to create transaction",
                    ListErrors = ["Repository returned null entity."]
                };
            }

            var createdEvent = new TransactionCreatedEvent(
                entity.Id,
                entityTransaction.CorrelationId,
                entityTransaction?.SourceAccountId,
                entityTransaction.Value
            );

            await _eventBus.PublishAsync("transactions-created", createdEvent);

            return new ResponseT<int>
            {
                IsSuccess = true,
                Data = entity.Id,
                Message = "Transaction accepted successfully"
            };

        }

        public async Task<ResponseT<int>> UpdateApprovedTransactionAsync(int id, Guid correlationId, Guid sourceAccountId, string message, CancellationToken cancellationToken = default)
        {
            return await UpdateTransactionStateAsync(id, correlationId, sourceAccountId, CurrentStateConstants.APPROVED, message, cancellationToken);
        }

        public async Task<ResponseT<int>> UpdateRejectedTransactionAsync(int id, Guid correlationId, Guid sourceAccountId, string message, CancellationToken cancellationToken = default)
        {
            return await UpdateTransactionStateAsync(id, correlationId, sourceAccountId, CurrentStateConstants.REJECTED, message, cancellationToken);
        }

        private async Task<ResponseT<int>> UpdateTransactionStateAsync(int id, Guid correlationId, Guid sourceAccountId, string newState, string message, CancellationToken cancellationToken)
        {
            var response = new ResponseT<int>();
            var entity = await _transactionRepository.UpdateCurrentStateAsync(id, newState, cancellationToken);

            return response;
        }
    }        
}
