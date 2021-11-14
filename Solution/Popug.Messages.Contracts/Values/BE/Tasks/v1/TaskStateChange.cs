namespace Popug.Messages.Contracts.Values.BE.Tasks.v1;

public record TaskStateChange(string TaskId, string PerformerId, DateTime Timestamp) : IEventValue
{
    public int Version => 1;
}