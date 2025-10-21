namespace Company.App.Domain.Events.ConfluentKafka  
{
    public class TransactionCreatedEvent
    {
        public int Id { get; set; } 
        public Guid CorrelationId { get; }
        public Guid? SourceAccountId { get; }
        public decimal Amount { get; }
        public DateTime CreatedAt { get; }

        public TransactionCreatedEvent(int id, Guid correlationId, Guid? sourceAccountId, decimal amount)
        {
            Id = id;
            CorrelationId = correlationId;
            SourceAccountId = sourceAccountId;
            Amount = amount;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
