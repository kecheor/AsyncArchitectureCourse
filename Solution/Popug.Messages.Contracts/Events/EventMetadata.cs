namespace Popug.Messages.Contracts.Events;
/// <summary>
/// Metadata attached to the values when transfered throw the message broker
/// </summary>
/// <param name="Id">Internal event id</param>
/// <param name="DataVersion">Data vesrion inside the message</param>
/// <param name="VersionName">Event name</param>
/// <param name="Timestamp">UTC timestamp when message was created</param>
/// <param name="Producer">Name of the producer that published the message</param>
public record EventMetadata (string Id, int DataVersion, string EventName, DateTime Timestamp, string Producer)
{
    public string ToTrace()
    {
        return $"Event: '{EventName}'-'{DataVersion}' Event Id: '{Id}'. Producer: '{Producer}. Timestamp: {Timestamp}'";
    }
}