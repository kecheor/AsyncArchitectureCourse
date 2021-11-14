using Popug.Common.Monads;
using Popug.Common.Monads.Errors;
using Popug.Messages.Contracts.Events;
using Popug.Messages.Contracts.Values;

namespace Popug.Messages.Contracts.Services
{
    public interface IEventValueSerializer
    {
        Either<string, Error> Serialize<TValue>(NewEventMessage<TValue> message) where TValue : IEventValue;
        Either<string, Error> Serialize<TValue>(string eventName, string producer, TValue value) where TValue : IEventValue;
        Either<EventMessage<TValue>, Error> Deserialize<TValue>(string json) where TValue : IEventValue;
    }
}
