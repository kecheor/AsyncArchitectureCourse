
using Popug.Common;
using Popug.Tasks.Repository.Models;

namespace Popug.Tasks.Repository
{
    public interface IPerformerRepository
    {
        Task<Performer?> Find(string chipId, CancellationToken cancellationToken);
        Task<IReadOnlyList<Performer>> FindInRole(Roles role, CancellationToken cancellationToken);
        Task<Performer?> Add(Performer account, CancellationToken cancellationToken);
        Task<IReadOnlyList<Performer>> GetAll(CancellationToken cancellationToken);
        Task<Performer?> Update(Performer account, CancellationToken cancellationToken);
        Task<int> LogAssign(TaskPerformerLog log, CancellationToken cancellationToken);
    }
}