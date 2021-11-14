namespace Popug.Messages.Contracts.Events;

public record EventMessage(EventMetadata Metadata, string Value);