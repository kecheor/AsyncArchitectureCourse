using Popug.Messages.Contracts.Values;
namespace Popug.Messages.Contracts.Events;
/// <summary>
/// Object containing value transfered throw message broker and attached metadata
/// </summary>
/// <typeparam name="TValue">Type of the value contained in the message</typeparam>
/// <param name="Metadata">Metadata attached to the message</param>
/// <param name="Value">Data value in the message</param>
public record EventMessage<TValue>(EventMetadata Metadata, TValue Value) where TValue : IEventValue;