using Popug.Common.Monads;
using Popug.Common.Services;
using Popug.Messages.Contracts.Events;
using Popug.Messages.Contracts.EventTypes.BE.Tasks;
using Popug.Messages.Contracts.EventTypes.CUD;
using Popug.Messages.Contracts.Services;
using Popug.Tasks.Repository.Models;

namespace Popug.Tasks.Repository
{
    internal class PerformersConsumer
    {
        //TODO:From configuration
        private const string TOPIC = "popug-accounts-stream";

        private readonly IPerformerRepository _accountRepository;
        private readonly IConsumer _accountConsumer;
        private readonly IJsonSerializer _jsonSerializer;

        public PerformersConsumer(IPerformerRepository accountRepository, IConsumer accountConsumer, IJsonSerializer jsonSerializer)
        {
            _accountRepository = accountRepository;
            _jsonSerializer = jsonSerializer;
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
                        var cudEvent = _accountConsumer.Consume(cancellationToken);
                        cudEvent.Apply(
                            async account => await ProcessAccountCUD(account, cancellationToken),
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

        private Either<Performer, ExceptionError> Deserialize(string data)
        {
            try
            {
                return _jsonSerializer.Deserialize<Performer>(data);
            }
            catch (Exception e)
            {
                return new ExceptionError(e);
            }
            
        }

        private async Task<Either<None, Error>> ProcessAccountCUD(ConsumedEvent consumed, CancellationToken cancellationToken)
        {
            var deserialized = Deserialize(consumed.SerializedValue);
            if(deserialized.HasError)
            {
                return deserialized.Error;
            }
            return await ProcessAccountCUD(consumed.Metadata, deserialized.Result, cancellationToken);
        }

        private async Task<Either<None, Error>> ProcessAccountCUD(EventMetadata metadata, Performer performer, CancellationToken cancellationToken)
        {
            switch (metadata.Name)
            {
                case CudEventType.Created:
                    await _accountRepository.Add(performer, cancellationToken);
                    return Of.None();
                case CudEventType.Updated:
                    await _accountRepository.Update(performer, cancellationToken);
                    return Of.None(); 
                case CudEventType.Deleted:
                default:
                    return new Error($"Unknown CUD event action {metadata.Name}");
            }
        }
    }
}
