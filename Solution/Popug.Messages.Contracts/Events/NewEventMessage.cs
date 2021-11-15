using Popug.Messages.Contracts.Values;
namespace Popug.Messages.Contracts.Events;
/// <summary>
/// Data object to submit value to the message broker
/// </summary>
/// <typeparam name="TValue">Type of the value</typeparam>
/// <param name="Topic"></param>
/// <param name="EventName"></param>
/// <param name="Producer"></param>
/// <param name="Value"></param>
public record NewEventMessage<TValue>(string Topic, string EventName, string Producer, TValue Value) where TValue : IEventValue;