using Popug.Common.Events;
using Popug.Common.Monads;
using Popug.Common.Services;
using Popug.Messages.Contracts.Events;
using Popug.Messages.Contracts.Services;

namespace Popug.Tasks.Repository
{
    internal class AccountsConsumer
    {
        //TODO:From configuration
        private const string TOPIC = "popug-accounts-stream";

        private readonly IAccountRepository _accountRepository;
        private readonly IConsumer _accountConsumer;
        private readonly IJsonSerializer _jsonSerializer;

        public AccountsConsumer(IAccountRepository accountRepository, IConsumer accountConsumer, IJsonSerializer jsonSerializer)
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

        private Either<Account, ExceptionError> Deserialize(string data)
        {
            try
            {
                return _jsonSerializer.Deserialize<Account>(data);
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

        private async Task<Either<None, Error>> ProcessAccountCUD(EventMetadata metadata, Account account, CancellationToken cancellationToken)
        {
            switch (metadata.Name)
            {
                case CudEventType.Created:
                    await _accountRepository.Add(account, cancellationToken);
                    return Of.None();
                case CudEventType.Updated:
                    await _accountRepository.Update(account, cancellationToken);
                    return Of.None(); 
                case CudEventType.Deleted:
                default:
                    return new Error($"Unknown CUD event action {metadata.Name}");
            }
        }
    }
}
