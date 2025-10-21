namespace Company.App.Application.Interface.Infrastructure.ConfluentKafka
{
    public interface IKafkaConsumer
    {
        Task ConsumeAsync<T>(Func<T, Task> handleMessage, CancellationToken token);
    }
}
