namespace Company.App.Domain.Events.ConfluentKafka
{
    public class TransactionResultEvent
    {
        public int Id { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid SourceAccountId { get; set; }
        public string EventMessage { get; set; }
    }   
}
