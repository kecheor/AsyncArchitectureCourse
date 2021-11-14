using Popug.Common.Monads;
using Popug.Tasks.Management.Models;

namespace Popug.Tasks.Management.Services
{
    public interface ITasksService
    {
        Task<Either<None, Error>> Close(string currentPopug, string taskId, CancellationToken cancellationToken);
        Task<Either<TaskDto, Error>> Create(string currentPopug, string taskDescription, CancellationToken cancellationToken);
        Task<Either<IReadOnlyList<TaskDto>, Error>> Mine(string currentPopug, CancellationToken cancellationToken);
        Task<Either<IReadOnlyList<StateChangeLog>, Error>> Reassign(string currentPopug, CancellationToken cancellationToken);
    }
}