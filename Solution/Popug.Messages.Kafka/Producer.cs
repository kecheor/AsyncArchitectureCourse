using Popug.Common.Monads;
using Popug.Common.Monads.Errors;
using Popug.Messages.Contracts.Services;
using Confluent.Kafka;
using Error = Popug.Common.Monads.Errors.Error;
using Popug.Messages.Contracts.Values;
using Popug.Messages.Contracts.Events;
using Popug.Common;

namespace Popug.Messages.Kafka
{
    public class Producer : IProducer
    {
        private readonly IProducer<Null, string> _producer;
        private readonly IEventValueSerializer _serializer;
        private readonly IMessageErrorLogger _messageErrorLogger;

        public Producer(IProducer<Null, string> producer)
        {
            _producer = producer;
        }

        public async Task<Either<None, Error>> Produce<TValue>(NewEventMessage<TValue> newEvent, CancellationToken cancellationToken) where TValue : IEventValue
        {
            var serialized = _serializer.Serialize(newEvent);
            if(serialized.HasError)
            {
                var text = $"Could not serialize message for {newEvent.EventName} event: {serialized.Error.ErrorMessage} in {newEvent.Producer}";
                await _messageErrorLogger.LogError(newEvent.Producer, text, null, null, default(CancellationToken));
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
                await _messageErrorLogger.LogError(newEvent.Producer, ex.Message, serialized.Result, ex, default(CancellationToken));
                return new Either<None, Error>(new ExceptionError(ex));
            }
            
            if(result.Status == PersistenceStatus.Persisted)
            {
                return new Either<None, Error>(Of.None());
            }
            else
            {
                await _messageErrorLogger.LogError(newEvent.Producer, "Could not commit message", serialized.Result, null, default(CancellationToken));
                return new Either<None, Error>(new Error("Something went wrong"));
            }
        }

        public void Dispose()
        {
            _producer.Dispose();
        }
    }
}
