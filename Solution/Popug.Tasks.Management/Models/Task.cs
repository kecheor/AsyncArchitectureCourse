using Popug.Common.Enums;

namespace Popug.Tasks.Management.Models;
public record TaskDto(string Id, string Description, string PerformerId, TaskState State, DateTime Created);