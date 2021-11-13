using Popug.Common.Monads;
using Popug.Common.Services;
using Popug.Messages.Contracts.Events;
using Popug.Messages.Contracts.Services;
namespace Popug.Messages.Kafka;

public class Consumer : IConsumer
{
    private readonly Confluent.Kafka.IConsumer<string, string> _consumer;
    private readonly IJsonSerializer _jsonSerializer;

    public Consumer(Confluent.Kafka.IConsumer<string, string> consumer, IJsonSerializer jsonSerializer)
    {
        _consumer = consumer;
        _jsonSerializer = jsonSerializer;
    }

    public void Subscribe(IEnumerable<string> topics)
    {
        _consumer.Subscribe(topics);
    }

    public void Subscribe(string topic)
    {
        _consumer.Subscribe(topic);
    }

    public void Unsubscribe()
    {
        _consumer.Unsubscribe();
    }

    public void Close()
    {
        _consumer.Close();
    }
    

    public Either<ConsumedEvent, Error> Consume(int millisecondsTimeout)
    {
        return ParseEvent(_consumer.Consume(millisecondsTimeout));
    }

    public Either<ConsumedEvent, Error> Consume(CancellationToken cancellationToken = default)
    {
        return ParseEvent(_consumer.Consume(cancellationToken));
    }

    public Either<ConsumedEvent, Error> Consume(TimeSpan timeout)
    {
        return ParseEvent(_consumer.Consume(timeout));
    }

    private Either<ConsumedEvent, Error> ParseEvent(Confluent.Kafka.ConsumeResult<string, string> consumeResult)
    {
        if(string.IsNullOrEmpty(consumeResult?.Message?.Value))
        {
            return Either<ConsumedEvent, Error>.Failure(new Error("Consumed data is empty"));
        }
        EventMessage value;
        try
        {
            //TODO: Event versioning
            value = _jsonSerializer.Deserialize<EventMessage>(consumeResult.Message.Value);
        }
        catch (Exception ex)
        {
            //TODO: Error handling
            return Either<ConsumedEvent, Error>.Failure(new ExceptionError(ex));
        }

        var result = new ConsumedEvent()
        {
            Topic = consumeResult.Topic,
            Metadata = value.Metadata,
            SerializedValue = value.Value
        };

        return Either<ConsumedEvent, Error>.Success(result);
    }

    public void Dispose()
    {
       _consumer.Dispose();
    }
}

