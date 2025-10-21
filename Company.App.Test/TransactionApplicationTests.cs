using Company.App.Application.Dto;
using Company.App.Application.Interface.Infrastructure.ConfluentKafka;
using Company.App.Application.Interface.Persistence.Repositories;
using Company.App.Application.UseCases.Transaction;
using Company.App.Domain;
using Company.App.Domain.Events.ConfluentKafka;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace Company.App.Test
{
    public class TransactionApplicationTests
    {
        private readonly Mock<ITransactionRepository> _repoMock;
        private readonly Mock<IKafkaProducer> _kafkaMock;
        private readonly Mock<IValidator<TransactionDto>> _validatorMock;
        private readonly Mock<IValidator<TransactionRequestDto>> _validatorRequestMock;
        private readonly TransactionApplication _service;

        public TransactionApplicationTests()
        {
            _repoMock = new Mock<ITransactionRepository>();
            _kafkaMock = new Mock<IKafkaProducer>();
            _validatorMock = new Mock<IValidator<TransactionDto>>();
            _validatorRequestMock = new Mock<IValidator<TransactionRequestDto>>();

            _service = new TransactionApplication(
                _repoMock.Object,
                _kafkaMock.Object,
                _validatorMock.Object,
                _validatorRequestMock.Object
            );
        }

        [Fact]
        public async Task GetTransactionAsync_ShouldReturnFail_WhenTransactionExternalIdIsEmpty()
        {
            // Arrange
            var request = new TransactionRequestDto
            {
                TransactionExternalId = Guid.Empty,
                CreatedAt = DateTime.UtcNow
            };

            _validatorRequestMock
                .Setup(v => v.ValidateAsync(It.IsAny<TransactionRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new[]
                {
                    new ValidationFailure("TransactionExternalId", "TransactionExternalId cannot be empty")
                }));

            // Act
            var result = await _service.GetTransactionAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Validation errors", result.Message);
            Assert.Contains("TransactionExternalId cannot be empty", result.ListErrors);

        }

        [Fact]
        public async Task GetTransactionAsync_ShouldReturnFail_WhenTransactionNotFound()
        {
            // Arrange
            var request = new TransactionRequestDto
            {
                TransactionExternalId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            };

            _validatorRequestMock
                .Setup(v => v.ValidateAsync(It.IsAny<TransactionRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _repoMock
                .Setup(r => r.FindByCorrelationIdAsync(request.TransactionExternalId, request.CreatedAt))
                .ReturnsAsync((FinancialTransaction)null);

            // Act
            var result = await _service.GetTransactionAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Transaction not found", result.Message);
        }

        [Fact]
        public async Task InsertTransactionAsync_ShouldReturnFail_WhenValidationFails()
        {
            // Arrange
            var transactionDto = new TransactionDto
            {
                SourceAccountId = Guid.NewGuid(),
                TargetAccountId = Guid.NewGuid(),
                TranferTypeId = 1,
                Value = 100
            };
            // Mock del validator para que devuelva errores
            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<TransactionDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new[]
                {
            new ValidationFailure("Value", "Value must be greater than zero")
                }));

            // Act
            var result = await _service.InsertTransactionAsync(transactionDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Validation errors", result.Message);
            Assert.Contains("Value must be greater than zero", result.ListErrors);

        }

        [Fact]
        public async Task InsertTransactionAsync_ShouldReturnFail_WhenRepositoryReturnsNull()
        {
            // Arrange
            var transactionDto = new TransactionDto
            {
                SourceAccountId = Guid.NewGuid(),
                TargetAccountId = Guid.NewGuid(),
                TranferTypeId = 1,
                Value = 100
            };

            // Validator pasa
            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<TransactionDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // Repositorio devuelve null
            _repoMock
                .Setup(r => r.AddEntityAsync(It.IsAny<FinancialTransaction>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((FinancialTransaction)null);

            // Act
            var result = await _service.InsertTransactionAsync(transactionDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Failed to create transaction", result.Message);
            Assert.Contains("Repository returned null entity.", result.ListErrors);
        }

        [Fact]
        public async Task InsertTransactionAsync_ShouldReturnSuccess_WhenTransactionCreated()
        {
            // Arrange
            var transactionDto = new TransactionDto
            {
                SourceAccountId = Guid.NewGuid(),
                TargetAccountId = Guid.NewGuid(),
                TranferTypeId = 1,
                Value = 100
            };

            var entity = new FinancialTransaction
            {
                Id = 123,
                CorrelationId = Guid.NewGuid(),
                SourceAccountId = transactionDto.SourceAccountId,
                TargetAccountId = transactionDto.TargetAccountId,
                TranferTypeId = transactionDto.TranferTypeId,
                Value = transactionDto.Value,
                CurrentState = "PENDING"
            };

            // Validator pasa
            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<TransactionDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // Repositorio devuelve la entidad creada
            _repoMock
                .Setup(r => r.AddEntityAsync(It.IsAny<FinancialTransaction>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            // Mock del evento Kafka
            _kafkaMock
                .Setup(e => e.PublishAsync("transactions-created", It.IsAny<TransactionCreatedEvent>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.InsertTransactionAsync(transactionDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(entity.Id, result.Data);
            Assert.Equal("Transaction created successfully", result.Message);

            // Verificar que PublishAsync fue llamado una vez
            _kafkaMock.Verify(e => e.PublishAsync("transactions-created", It.IsAny<TransactionCreatedEvent>()), Times.Once);
        }
    }
}