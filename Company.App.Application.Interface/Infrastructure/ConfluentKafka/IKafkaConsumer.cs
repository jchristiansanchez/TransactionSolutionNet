namespace Company.App.Application.Interface.Infrastructure.ConfluentKafka
{
    public interface IKafkaConsumer
    {
        Task ConsumeAsync<T>(string topic, Func<T, Task> handleMessage, CancellationToken token);
    }
}
