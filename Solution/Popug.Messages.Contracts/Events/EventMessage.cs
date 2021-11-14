using Popug.Messages.Contracts.Values;
namespace Popug.Messages.Contracts.Events;
public record EventMessage<TValue>(EventMetadata Metadata, TValue Value) where TValue : IEventValue;