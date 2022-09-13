using Microsoft.Extensions.Logging;
using Popug.Common.Monads;
using Popug.Common.Monads.Errors;
using Popug.Messages.Contracts.Events;
using Popug.Messages.Contracts.EventTypes.CUD;
using Popug.Messages.Contracts.Services;
using Popug.Messages.Contracts.Values.CUD.Popugs;
using Popug.Tasks.Repository.Models;
using Popug.Tasks.Repository.Repositories;

namespace Popug.Tasks.Consumer
{
    internal class PerformersConsumer
    {
        //TODO:From configuration
        private const string TOPIC = "popug-accounts-stream";

        private readonly IPerformerRepository _accountRepository;
        private readonly IConsumer _accountConsumer;
        private readonly ILogger<PerformersConsumer> _logger;

        public PerformersConsumer(IPerformerRepository accountRepository, IConsumer accountConsumer, ILogger<PerformersConsumer> logger)
        {
            _accountRepository = accountRepository;
            _accountConsumer = accountConsumer;
            _logger = logger;
        }

        public void Run(CancellationToken cancellationToken)
        {
            try
            {
                _accountConsumer.Subscribe(TOPIC);
                while (true)
                {
                    try
                    {
                        var cudEvent = _accountConsumer.Consume<PopugValue>(cancellationToken);
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
                _accountConsumer.Close();
            }
        }

        private async Task<Either<None, Error>> ProcessAccountCUD(EventMessage<PopugValue> consumed, CancellationToken cancellationToken)
        {
            _logger.LogTrace($"Consumed account CUD event from {TOPIC}: {consumed.Metadata.ToTrace()}");
            var performer = new Performer(null, consumed.Value.ChipId, consumed.Value.Name, consumed.Value.Role, DateTime.UtcNow);
            switch (consumed.Metadata.EventName)
            {
                case CudEventType.Created:
                    _logger.LogInformation($"Creating new account {performer.ChipId}:{performer.Name} with role {performer.Role}");
                    await _accountRepository.Add(performer, cancellationToken);
                    return Of.None();
                case CudEventType.Updated:
                    _logger.LogInformation($"Updating account {performer.ChipId}:{performer.Name} with role {performer.Role}");
                    await _accountRepository.Update(performer, cancellationToken);
                    return Of.None();
                case CudEventType.Deleted:
                default:
                    var message = $"Unknown CUD event action {consumed.Metadata.EventName}";
                    _logger.LogInformation(message);
                    return new Error(message);
            }
        }
    }
}
