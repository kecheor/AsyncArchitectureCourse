namespace Popug.Messages.Contracts.Values.BE.Tasks.v1;

/// <summary>
/// First version for bisiness event value for task state change
/// </summary>
/// <param name="TaskId">Public id of the changed task</param>
/// <param name="PerformerId">Public id of the current task performer</param>
/// <param name="Timestamp">UTC timestamp of the action</param>
public record TaskStateChange(string TaskId, string PerformerId, DateTime Timestamp) : IEventValue
{
    public int Version => 1;
}