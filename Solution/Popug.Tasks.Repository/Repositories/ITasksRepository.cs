using Popug.Common;
using Popug.Tasks.Repository.Models;

namespace Popug.Tasks.Repository.Repositories
{
    public interface ITasksRepository
    {
        Task<int> Add(TaskData task, CancellationToken cancellationToken);
        Task<IReadOnlyList<TaskData>> FindByPerformer(string performer, TaskState state, CancellationToken cancellationToken);
        Task<TaskData?> FindByPublicId(string taskId, CancellationToken cancellationToken);
        Task<IReadOnlyList<TaskData>> FindByState(TaskState state, CancellationToken cancellationToken);
        Task<int> Save(TaskData task, CancellationToken cancellationToken);
    }
}