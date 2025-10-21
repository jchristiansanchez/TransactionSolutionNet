using Company.App.Application.Interface.Infrastructure.ConfluentKafka;
using Confluent.Kafka;
using System.Text.Json;

namespace Company.App.Infrastructure.ConfluentKafka
{
    public class KafkaProducer : IKafkaProducer 
    {
        private readonly IProducer<Null, string> _producer;

        public KafkaProducer(IProducer<Null, string> producer)
        {
            _producer = producer;
        }

        public async Task PublishAsync<T>(string topic, T message)
        {
            var json = JsonSerializer.Serialize(message);

            await _producer.ProduceAsync(topic, new Message<Null, string> { Value = json });
        }   
    }
}
