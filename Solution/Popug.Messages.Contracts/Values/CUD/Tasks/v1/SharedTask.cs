using Popug.Common;

namespace Popug.Messages.Contracts.Values.CUD.Tasks.v1;
public record SharedTask(string TaskId, string Description, string PerformerId, TaskState State, DateTime Created);