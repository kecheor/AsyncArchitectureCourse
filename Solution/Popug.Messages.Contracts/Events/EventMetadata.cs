namespace Popug.Messages.Contracts.Events;
public record EventMetadata (string Id, int Version, string Name, DateTime Timestamp, string Producer);