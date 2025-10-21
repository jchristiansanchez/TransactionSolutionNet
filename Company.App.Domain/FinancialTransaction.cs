using Company.App.Domain.Common;

namespace Company.App.Domain
{
    public class FinancialTransaction: BaseAuditableEntity  
    {
        public Guid CorrelationId { get; set; } 
        public Guid? SourceAccountId { get; set; }

        public Guid? TargetAccountId { get; set; }

        public int TranferTypeId { get; set; }

        public decimal Value { get; set; }

        public string CurrentState { get; set; }

    }
}   
