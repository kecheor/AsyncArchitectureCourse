using Popug.Common.Monads;
using Popug.Common.Monads.Errors;
using Popug.Messages.Contracts.Services;
using Confluent.Kafka;
using Error = Popug.Common.Monads.Errors.Error;
using Popug.Messages.Contracts.Values;
using Popug.Messages.Contracts.Events;

namespace Popug.Messages.Kafka
{
    public class Producer : IProducer
    {
        private readonly IProducer<Null, string> _producer;
        private readonly IEventValueSerializer _serializer;

        public Producer(IProducer<Null, string> producer)
        {
            _producer = producer;
        }

        public async Task<Either<None, Error>> Produce<TValue>(NewEventMessage<TValue> newEvent, CancellationToken cancellationToken) where TValue : IEventValue
        {
            var serialized = _serializer.Serialize(newEvent);
            if(serialized.HasError)
            {
                return serialized.Error;
            }

            var message = new Message<Null, string>()
            {
                Value = serialized.Result
            };
            DeliveryResult<Null, string> result;
            try
            {
                //For some unknown reason awaiting does not return
                var task = _producer.ProduceAsync(newEvent.Topic, message, cancellationToken);
                _producer.Flush();
                await Task.Delay(1000);
                result = task.Result;
            }
            catch (Exception ex)
            {
                return new Either<None, Error>(new ExceptionError(ex));
            }
            
            if(result.Status == PersistenceStatus.Persisted)
            {
                return new Either<None, Error>(Of.None());
            }
            else
            {
                //TODO:Error handling
                return new Either<None, Error>(new Error("Something went wrong"));
            }
        }

        public void Dispose()
        {
            //_producer.Dispose();
        }
    }
}
