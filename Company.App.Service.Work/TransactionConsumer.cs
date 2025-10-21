using Company.App.Application.Interface.Infrastructure.ConfluentKafka;
using Company.App.Application.Interface.UseCases;
using Company.App.Domain.Events.ConfluentKafka;

namespace Company.App.Service.Work
{
    public class TransactionConsumer : BackgroundService
    {

        private readonly IKafkaConsumerApproved _approvedConsumer;
        private readonly IKafkaConsumerRejected _rejectedConsumer;
        private readonly ILogger<TransactionConsumer> _logger;
        private readonly IServiceProvider _serviceProvider;

        public TransactionConsumer(
            IKafkaConsumerApproved approvedConsumer,
            IKafkaConsumerRejected rejectedConsumer,
            IServiceProvider serviceProvider,
            ILogger<TransactionConsumer> logger)
        {
            _approvedConsumer = approvedConsumer;
            _rejectedConsumer = rejectedConsumer;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var rejectedTask = Task.Run(() => ConsumeTopic(_rejectedConsumer, "transactions-rejected", false, stoppingToken), stoppingToken);
            var approvedTask = Task.Run(() => ConsumeTopic(_approvedConsumer, "transactions-approved", true, stoppingToken), stoppingToken);

            await Task.WhenAll(rejectedTask, approvedTask);
        }

        private async Task ConsumeTopic(IKafkaConsumer consumer, string topic, bool isApproved, CancellationToken stoppingToken)
        {
            await consumer.ConsumeAsync<TransactionResultEvent>(
                topic,
                async evt =>
                {
                    using var scope = _serviceProvider.CreateScope();
                    var transactionApp = scope.ServiceProvider.GetRequiredService<ITransactionApplication>();

                    if (isApproved)
                        await transactionApp.UpdateApprovedTransactionAsync(evt.Id, evt.CorrelationId, evt.SourceAccountId, evt.EventMessage);
                    else
                        await transactionApp.UpdateRejectedTransactionAsync(evt.Id, evt.CorrelationId, evt.SourceAccountId, evt.EventMessage);
                },
                stoppingToken
            );
        }

    }
}
