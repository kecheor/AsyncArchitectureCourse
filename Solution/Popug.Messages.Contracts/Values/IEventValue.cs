namespace Popug.Messages.Contracts.Values;
/// <summary>
/// Marker interface for values serialized to message brocker.
/// </summary>
public interface IEventValue
{
    int Version { get; }
}