using Popug.Common.Monads;
using Popug.Common.Monads.Errors;
using Popug.Messages.Contracts.Events;
using Popug.Messages.Contracts.EventTypes.CUD;
using Popug.Messages.Contracts.Services;
using Popug.Messages.Contracts.Values.CUD.Popugs;
using Popug.Tasks.Repository.Models;

namespace Popug.Tasks.Repository
{
    internal class PerformersConsumer
    {
        //TODO:From configuration
        private const string TOPIC = "popug-accounts-stream";

        private readonly IPerformerRepository _accountRepository;
        private readonly IConsumer _accountConsumer;

        public PerformersConsumer(IPerformerRepository accountRepository, IConsumer accountConsumer)
        {
            _accountRepository = accountRepository;
            _accountConsumer = accountConsumer;
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
                            //TODO:Logging
                            err => Task.FromResult(0));
                    }
                    catch (Confluent.Kafka.ConsumeException e)
                    {
                        //TODO:Logging
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
            var performer = new Performer(null, consumed.Value.ChipId, consumed.Value.Name, consumed.Value.Role, DateTime.UtcNow);
            switch (consumed.Metadata.EventName)
            {
                case CudEventType.Created:
                    await _accountRepository.Add(performer, cancellationToken);
                    return Of.None();
                case CudEventType.Updated:
                    await _accountRepository.Update(performer, cancellationToken);
                    return Of.None(); 
                case CudEventType.Deleted:
                default:
                    return new Error($"Unknown CUD event action {consumed.Metadata.EventName}");
            }
        }
    }
}
