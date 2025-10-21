using Company.App.Application.Interface.Infrastructure.ConfluentKafka;
using Company.App.Infrastructure.ConfluentKafka;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Company.App.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructureInjectionServices(this IServiceCollection services, IConfiguration config, IHostEnvironment env)
        {            
            services.AddSingleton<IKafkaConsumerApproved>(sp =>
                new KafkaConsumer("localhost:9092", "transactions-approved-group", "transactions-approved"));

            services.AddSingleton<IKafkaConsumerRejected>(sp =>
                new KafkaConsumer("localhost:9092", "transactions-rejected-group", "transactions-rejected"));

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = config["Kafka:BootstrapServers"] ?? "localhost:9092"
            };

            var producer = new ProducerBuilder<Null, string>(producerConfig).Build();

            services.AddSingleton<IKafkaProducer>(new KafkaProducer(producer));
           
            return services;    
        }
    }
}
