namespace Popug.Messages.Contracts.Events;
/// <summary>
/// Event message with value serialized to json and prepared for transfer
/// </summary>
/// <param name="Metadata"></param>
/// <param name="Value"></param>
internal record SerializedEventMessage(EventMetadata Metadata, string Value);