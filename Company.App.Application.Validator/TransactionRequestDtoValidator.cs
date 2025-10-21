using Company.App.Application.Dto;
using FluentValidation;

namespace Company.App.Application.Validator
{
    public class TransactionRequestDtoValidator : AbstractValidator<TransactionRequestDto>
    {
        public TransactionRequestDtoValidator()
        {
            RuleFor(x => x.TransactionExternalId)
                .NotEmpty().WithMessage("TransactionExternalId cannot be empty");

            RuleFor(x => x.CreatedAt)
                .NotEmpty().WithMessage("CreatedAt is required")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("CreatedAt cannot be in the future");
        }
    }
}
