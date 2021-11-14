using Microsoft.EntityFrameworkCore;
using Popug.Common;
using Popug.Tasks.Repository.Models;

namespace Popug.Tasks.Repository.Repositories
{
    public class TasksRepository : ITasksRepository
    {
        private readonly TasksDbContext _dbContext;

        public TasksRepository(TasksDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<TaskData>> FindByPerformer(string performer, TaskState state, CancellationToken cancellationToken)
        {
            return await _dbContext.Tasks
                .Where(t => t.Performer.ChipId == performer && t.Status == state)
                .Include(t => t.Performer)
                .ToArrayAsync(cancellationToken);
        }

        public async Task<TaskData?> FindByPublicId(string taskId, CancellationToken cancellationToken)
        {
            return await _dbContext.Tasks.Include(t => t.Performer).FirstOrDefaultAsync(t => t.TaskPublicId == taskId, cancellationToken);
        }

        public async Task<IReadOnlyList<TaskData>> FindByState(TaskState state, CancellationToken cancellationToken)
        {
            return await _dbContext.Tasks
                .Where(t => t.Status == state)
                .ToArrayAsync(cancellationToken);
        }

        public async Task<int> Add(TaskData task, CancellationToken cancellationToken)
        {
            _dbContext.Tasks.Add(task);
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> Save(TaskData task, CancellationToken cancellationToken)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
