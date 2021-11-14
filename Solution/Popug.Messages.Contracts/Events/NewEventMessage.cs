using Popug.Messages.Contracts.Values;
namespace Popug.Messages.Contracts.Events;
public record NewEventMessage<TValue>(string Topic, string EventName, string Producer, TValue Value) where TValue : IEventValue;