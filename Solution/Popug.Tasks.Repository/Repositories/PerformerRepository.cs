using Microsoft.EntityFrameworkCore;
using Popug.Common.Enums;
using Popug.Tasks.Repository.Models;

namespace Popug.Tasks.Repository.Repositories
{
    public class PerformerRepository : IPerformerRepository
    {
        private readonly TasksDbContext _dbContext;
        public PerformerRepository(TasksDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<Performer>> GetAll(CancellationToken cancellationToken)
        {
            var records = await _dbContext.Performer.ToArrayAsync(cancellationToken);
            return records?.ToArray() ?? Array.Empty<Performer>();
        }

        public async Task<Performer?> Find(string chipId, CancellationToken cancellationToken)
        {
            return await _dbContext.Performer.FirstOrDefaultAsync(a => a.ChipId == chipId, cancellationToken);
        }

        public async Task<Performer?> Add(Performer performer, CancellationToken cancellationToken)
        {
            var exising = await _dbContext.Performer.FirstOrDefaultAsync(a => a.ChipId == performer.ChipId, cancellationToken);
            if (exising != null)
            {
                throw new ArgumentException("Such popug already exisits");
            }
            _dbContext.Add(performer with { Id = null, Created = DateTime.UtcNow });
            await _dbContext.SaveChangesAsync();
            return performer;
        }

        public async Task<Performer?> Update(Performer performer, CancellationToken cancellationToken)
        {
            if (!performer.Id.HasValue)
            {
                return null;
            }

            var record = await _dbContext.Performer.FirstOrDefaultAsync(a => a.Id == performer.Id, cancellationToken);
            if (record == null)
            {
                return null;
            }
            record = record with { ChipId = performer.ChipId, Name = performer.Name, Role = performer.Role };
            await _dbContext.SaveChangesAsync();
            return record;
        }


        public async Task<IReadOnlyList<Performer>> FindInRole(Roles role, CancellationToken cancellationToken)
        {
            return await _dbContext.Performer.Where(p => p.Role == role).ToArrayAsync(cancellationToken);
        }

        public async Task<int> LogAssign(TaskPerformerLog log, CancellationToken cancellationToken)
        {
            _dbContext.PerformerLogs.Add(log);
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
