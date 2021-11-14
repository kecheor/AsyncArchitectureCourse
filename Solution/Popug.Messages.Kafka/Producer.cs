using Popug.Common.Monads;
using Popug.Common.Services;
using Popug.Messages.Contracts.Events;
using Popug.Messages.Contracts.Services;

namespace Popug.Messages.Kafka
{
    public class Producer : IProducer
    {
        private readonly Confluent.Kafka.IProducer<string, string> _producer;
        private readonly IJsonSerializer _jsonSerializer;

        public Producer(Confluent.Kafka.IProducer<string, string> producer, IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
            _producer = producer;
        }

        public async Task<Either<None, Error>> Produce(string topic, EventMetadata metadata, string value, CancellationToken cancellationToken)
        {
            //TODO:Validation
            var payload = new EventMessage(metadata, value);
            var json = _jsonSerializer.Serialize(payload);
            var message = new Confluent.Kafka.Message<string, string>()
            {
                Key = metadata.Id,
                Value = json
            };
            Confluent.Kafka.DeliveryResult<string, string> result;
            try
            {
                //For some unknown reason awaiting does not return
                var task = _producer.ProduceAsync(topic, message, cancellationToken);
                _producer.Flush();
                result = task.Result;
            }
            catch (Exception ex)
            {
                return new Either<None, Error>(new ExceptionError(ex));
            }
            
            if(result.Status == Confluent.Kafka.PersistenceStatus.Persisted)
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
