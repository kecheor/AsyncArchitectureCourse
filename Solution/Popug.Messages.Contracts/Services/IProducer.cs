using Popug.Common.Monads;
using Popug.Common.Monads.Errors;
using Popug.Messages.Contracts.Events;
using Popug.Messages.Contracts.Values;

namespace Popug.Messages.Contracts.Services;
public interface IProducer : IDisposable
{
    Task<Either<None, Error>> Produce<TValue>(NewEventMessage<TValue> newEvent, CancellationToken cancellationToken) where TValue : IEventValue;
}
