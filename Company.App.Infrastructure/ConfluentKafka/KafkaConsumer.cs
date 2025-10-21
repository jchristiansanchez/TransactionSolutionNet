using Company.App.Application.Interface.Infrastructure.ConfluentKafka;
using Confluent.Kafka;
using System.Text.Json;

namespace Company.App.Infrastructure.ConfluentKafka
{
    //public class KafkaConsumer : IKafkaConsumer    
    public class KafkaConsumer : IKafkaConsumerApproved, IKafkaConsumerRejected

    {
        private readonly IConsumer<string, string> _consumer;
        private readonly string _topic;

        public KafkaConsumer(string bootstrapServers, string groupId, string topic)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<string, string>(config).Build();
            _topic = topic;
        }

        public async Task ConsumeAsync<T>(Func<T, Task> handleMessage, CancellationToken token)
        {
            _consumer.Subscribe(_topic);


            while (!token.IsCancellationRequested)
            {
                try
                {
                    var cr = _consumer.Consume(token);

                    var message = JsonSerializer.Deserialize<T>(cr.Message.Value);
                    if (message != null)
                        await handleMessage(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error en topic {_topic}: {ex.Message}");
                }
            }
        }

    }
}
