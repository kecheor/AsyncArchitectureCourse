using Popug.Common.Enums;

namespace Popug.Messages.Contracts.Values.CUD.Tasks.v1;
/// <summary>
/// First version of CUD event value for task
/// </summary>
/// <param name="TaskId">Public task id</param>
/// <param name="Description">Task text</param>
/// <param name="PerformerId">Public id of currently assighed popug</param>
/// <param name="State">Current task state</param>
/// <param name="Created">UTC timestamp of task creation</param>
[Obsolete("Please switch to v2")]
public record TaskValue(string TaskId, string Description, string PerformerId, TaskState State, DateTime Created) : IEventValue
{
    public int Version => 1;
}