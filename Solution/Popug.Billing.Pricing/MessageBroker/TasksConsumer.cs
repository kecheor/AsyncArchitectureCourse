using Microsoft.Extensions.Logging;
using Popug.Common.Monads;
using Popug.Common.Monads.Errors;
using Popug.Messages.Contracts.Events;
using Popug.Messages.Contracts.EventTypes.BE.Tasks;
using Popug.Messages.Contracts.EventTypes.CUD;
using Popug.Messages.Contracts.Services;
using Popug.Messages.Contracts.Values.CUD.Popugs;

namespace Popug.Billing.Pricing.MessageBroker
{
    internal class TasksConsumer
    {
        //TODO:From configuration
        private static string TOPIC = "popug-tasks-events";

        private readonly IConsumer _tasksConsumer;
        private readonly ILogger<TasksConsumer> _logger;

        public TasksConsumer(IConsumer consumer, ILogger<TasksConsumer> logger)
        {
            _tasksConsumer = consumer;
            _logger = logger;
        }

        public void Run(CancellationToken cancellationToken)
        {
            try
            {
                _tasksConsumer.Subscribe(TOPIC);
                while (true)
                {
                    try
                    {
                        var cudEvent = _tasksConsumer.Consume<PopugValue>(cancellationToken);
                        cudEvent.Apply(
                            async p => await ProcessAccountCUD(p, cancellationToken),
                            err => Task.FromResult(0));
                    }
                    catch (Confluent.Kafka.ConsumeException e)
                    {
                        _logger.LogError(e, $"Error consuming event from {TOPIC}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _tasksConsumer.Close();
            }
        }

        private async Task<Either<None, Error>> ProcessAccountCUD(EventMessage<PopugValue> consumed, CancellationToken cancellationToken)
        {
            _logger.LogTrace($"Consumed tasks BE event from {TOPIC}: {consumed.Metadata.ToTrace()}");
            switch (consumed.Metadata.EventName)
            {
                case TaskBusinessEvent.Assigned:

                case TaskBusinessEvent.Closed:

                default:
                    var message = $"Unknown CUD event action {consumed.Metadata.EventName}";
                    _logger.LogInformation(message);
                    return new Error(message);
            }
        }
    }
}
