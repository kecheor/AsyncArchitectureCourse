using Microsoft.EntityFrameworkCore;

namespace Popug.Tasks.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly TasksDbContext _dbContext;
        public AccountRepository(TasksDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<Account>> GetAll(CancellationToken cancellationToken)
        {
            var records = await _dbContext.Accounts.ToArrayAsync(cancellationToken);
            return records?.ToArray() ?? Array.Empty<Account>();
        }

        public async Task<Account?> Add(Account account, CancellationToken cancellationToken)
        {
            var exising = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.ChipId == account.ChipId, cancellationToken);
            if (exising != null)
            {
                throw new ArgumentException("Such popug already exisits");
            }
            _dbContext.Add(account with { Id = null });
            await _dbContext.SaveChangesAsync();
            return account;
        }

        public async Task<Account?> Update(Account account, CancellationToken cancellationToken)
        {
            if (!account.Id.HasValue)
            {
                return null;
            }

            var record = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == account.Id, cancellationToken);
            if (record == null)
            {
                return null;
            }
            record = record with { ChipId = account.ChipId, Name = account.Name, Role = account.Role };
            await _dbContext.SaveChangesAsync();
            return record;
        }
    }
}
