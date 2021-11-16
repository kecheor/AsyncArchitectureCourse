using Popug.Common;
using Popug.Common.Monads;
using Popug.Common.Monads.Errors;
using Popug.Messages.Contracts.Events;
using Popug.Messages.Contracts.Services;
using Popug.Messages.Contracts.Values;

namespace Popug.Messages.Kafka;

public class Consumer : IConsumer
{
    private readonly Confluent.Kafka.IConsumer<Confluent.Kafka.Null, string> _consumer;
    private readonly IEventValueSerializer _serializer;
    private readonly IMessageErrorLogger _messageErrorLogger;

    public Consumer(Confluent.Kafka.IConsumer<Confluent.Kafka.Null, string> consumer, IEventValueSerializer serializer)
    {
        _consumer = consumer;
        _serializer = serializer;
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
    

    public Either<EventMessage<TValue>, Error> Consume<TValue>(int millisecondsTimeout) where TValue : IEventValue
    {
        var message = _consumer.Consume(millisecondsTimeout);
        return ParseMessage<TValue>(message);
    }

    public Either<EventMessage<TValue>, Error> Consume<TValue>(CancellationToken cancellationToken = default) where TValue : IEventValue
    {
        var message = _consumer.Consume(cancellationToken);
        return ParseMessage<TValue>(message);
    }

    public Either<EventMessage<TValue>, Error> Consume<TValue>(TimeSpan timeout) where TValue : IEventValue
    {
        var message = _consumer.Consume(timeout);
        return ParseMessage<TValue>(message);
    }

    private Either<EventMessage<TValue>, Error> ParseMessage<TValue>(Confluent.Kafka.ConsumeResult<Confluent.Kafka.Null, string> consumeResult) where TValue : IEventValue
    {
        var result = _serializer.Deserialize<TValue>(consumeResult.Message.Value);
        if(!result.HasError)
        {
            return result.Result;
        }
        _messageErrorLogger.LogError(consumeResult.Topic,
            $"Could not consume message with offsert ${consumeResult.Offset}: {result.Error.ErrorMessage}",
            consumeResult.Message.Value,
            (result.Error as ExceptionError)?.Exception,
            default(CancellationToken));
        return result.Error;
    }

    public void Dispose()
    {
       _consumer.Dispose();
    }
}

