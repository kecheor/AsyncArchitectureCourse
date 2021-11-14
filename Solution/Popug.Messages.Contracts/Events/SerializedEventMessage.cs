namespace Popug.Messages.Contracts.Events;
internal record SerializedEventMessage(EventMetadata Metadata, string Value);