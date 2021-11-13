namespace Popug.Messages.Contracts.Events;

public class ConsumedEvent
{
    public string Topic { init; get; }

    public EventMetadata Metadata { init; get; }

    public string SerializedValue { init; get; }
}

