using Popug.Common.Monads;
using Popug.Messages.Contracts.Events;
using Popug.Messages.Contracts.Values;
using Error = Popug.Common.Monads.Errors.Error;

namespace Popug.Messages.Contracts.Services;
public interface IConsumer : IDisposable
{
    Either<EventMessage<TValue>, Error> Consume<TValue>(int millisecondsTimeout) where TValue : IEventValue;

    Either<EventMessage<TValue>, Error> Consume<TValue>(CancellationToken cancellationToken = default(CancellationToken)) where TValue : IEventValue;

    Either<EventMessage<TValue>, Error> Consume<TValue>(TimeSpan timeout) where TValue : IEventValue;

    void Subscribe(IEnumerable<string> topics);

    void Subscribe(string topic);

    void Unsubscribe();

    void Close();
}
