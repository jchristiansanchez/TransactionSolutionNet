namespace Company.App.Application.Interface.Infrastructure.ConfluentKafka
{
    public interface IKafkaProducer
    {
        Task PublishAsync<T>(string topic, T message);  
    }
}
