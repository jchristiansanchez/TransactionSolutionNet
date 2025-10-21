using Company.App.Application.Dto;
using FluentValidation;

namespace Company.App.Application.Validator
{
    public class TransactionDtoValidator : AbstractValidator<TransactionDto>
    {
        public TransactionDtoValidator()
        {
            RuleFor(x => x.SourceAccountId)
                .NotNull().WithMessage("SourceAccountId is required.")
                .NotEqual(Guid.Empty).WithMessage("SourceAccountId must be a valid GUID.");

            RuleFor(x => x.TargetAccountId)
                .NotNull().WithMessage("TargetAccountId is required.")
                .NotEqual(Guid.Empty).WithMessage("TargetAccountId must be a valid GUID.");


            RuleFor(x => x.TranferTypeId)
                .GreaterThan(0).WithMessage("TransferTypeId must be valid.");

            RuleFor(x => x.Value)
                .GreaterThan(0).WithMessage("Value must be greater than 0.");

        }
    }
}
