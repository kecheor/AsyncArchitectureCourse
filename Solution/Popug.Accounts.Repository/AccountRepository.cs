using Microsoft.EntityFrameworkCore;

namespace Popug.Accounts.Repository
{

    public class AccountRepository : IAccountRepository
    {
        private readonly AccountsDbContext _dbContext;
        public AccountRepository(AccountsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<Account>> GetAll()
        {
            var records = await _dbContext.Accounts.ToArrayAsync();
            return records?.ToArray() ?? Array.Empty<Account>();
        }

        public async Task<Account?> Add(Account account)
        {
            var exising = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.ChipId == account.ChipId || a.BeakCurvature == account.BeakCurvature);
            if (exising != null)
            {
                throw new ArgumentException("Such popug already exisits");
            }
            _dbContext.Add(account with { Id = null });
            await _dbContext.SaveChangesAsync();
            return account;
        }

        public async Task<Account?> Update(Account account)
        {
            if (!account.Id.HasValue)
            {
                return null;
            }

            var record = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == account.Id);
            if (record == null)
            {
                return null;
            }
            record = record with { ChipId = account.ChipId, Name = account.Name, Role = account.Role, BeakCurvature = account.BeakCurvature };
            await _dbContext.SaveChangesAsync();
            return record;
        }


        public async Task<Account?> Find(int curvature)
        {
            return await _dbContext.Accounts.FirstOrDefaultAsync(a => a.BeakCurvature == curvature);
        }
    }
}
