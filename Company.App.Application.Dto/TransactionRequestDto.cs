namespace Company.App.Application.Dto
{
    public class TransactionRequestDto
    {
        public Guid? TransactionExternalId { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
