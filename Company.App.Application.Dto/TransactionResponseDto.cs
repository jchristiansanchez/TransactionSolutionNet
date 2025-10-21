namespace Company.App.Application.Dto
{
    public class TransactionResponseDto
    {
        public Guid TransactionExternalId { get; set; }
        public Guid? SourceAccountId { get; set; }
        public Guid? TargetAccountId { get; set; }
        public int TranferTypeId { get; set; }
        public decimal Value { get; set; }
        public string CurrentState { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
