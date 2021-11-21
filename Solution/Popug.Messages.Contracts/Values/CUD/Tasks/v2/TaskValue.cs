using Popug.Common.Enums;

namespace Popug.Messages.Contracts.Values.CUD.Tasks.v2;

/// <summary>
/// Second version of CUD event value for task
/// </summary>
/// <param name="TaskId">Public task id</param>
/// <param name="JiraId">Optional jira identifier for the task</param>
/// <param name="Title">Task text</param>
/// <param name="PerformerId">Public id of currently assighed popug</param>
/// <param name="State">Current task state</param>
/// <param name="Created">UTC timestamp of task creation</param>
public record TaskValue(string TaskId, string JiraId, string Title, string PerformerId, TaskState State, DateTime Created) : IEventValue
{
    public int Version => 2;
}