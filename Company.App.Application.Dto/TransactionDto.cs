namespace Company.App.Application.Dto
{
    public class TransactionDto 
    {
        public Guid? SourceAccountId { get; set; }

        public Guid? TargetAccountId { get; set; }

        public int TranferTypeId { get; set; }

        public decimal Value { get; set; }
    }
}
