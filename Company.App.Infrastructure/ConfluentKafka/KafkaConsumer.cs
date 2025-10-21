using Company.App.Application.Interface.Infrastructure.ConfluentKafka;
using Company.App.Domain.Events.ConfluentKafka;
using Confluent.Kafka;
using System.Text.Json;

namespace Company.App.Infrastructure.ConfluentKafka
{
    //public class KafkaConsumer : IKafkaConsumer    
    public class KafkaConsumer : IKafkaConsumerApproved, IKafkaConsumerRejected

    {
        private readonly IConsumer<string, string> _consumer;

        public KafkaConsumer(string bootstrapServers, string groupId)  
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<string, string>(config).Build();
        }

        public async Task ConsumeAsync<T>(string topic, Func<T, Task> handleMessage, CancellationToken token)
        {
            _consumer.Subscribe(topic);

            while (!token.IsCancellationRequested)
            {
                try
                {
                    var cr = _consumer.Consume(token);
                    Console.WriteLine($"Mensaje recibido: {cr.Message.Value}");

                    var message = JsonSerializer.Deserialize<T>(cr.Message.Value);
                    if (message != null)
                    {
                        await handleMessage(message);
                    }
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error deserializando mensaje: {ex.Message}");
                }
                catch (ConsumeException ex)
                {
                    Console.WriteLine($"Error consumiendo Kafka: {ex.Error.Reason}");
                }
            }
        }
    }
}
