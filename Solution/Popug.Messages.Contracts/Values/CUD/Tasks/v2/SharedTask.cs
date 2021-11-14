using Popug.Common;

namespace Popug.Messages.Contracts.Values.CUD.Tasks.v2;
public record SharedTask(string TaskId, string JiraId, string Title, string PerformerId, TaskState State, DateTime Created) : IEventValue
{
    public int Version => 2;
}