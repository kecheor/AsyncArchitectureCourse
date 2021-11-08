using Microsoft.EntityFrameworkCore;

namespace Popug.Tasks.Repository
{

    public class AccountRepository : IAccountRepository
    {
        public async Task<IReadOnlyList<Account>> GetAll()
        {
            using var db = new TasksDbContext();
            var records = await db.Accounts.ToArrayAsync();
            return records?.ToArray() ?? Array.Empty<Account>();
        }

        public async Task<Account?> Add(Account account)
        {
            using var db = new TasksDbContext();
            var exising = await db.Accounts.FirstOrDefaultAsync(a => a.ChipId == account.ChipId);
            if(exising != null)
            {
                throw new ArgumentException("Such popug already exisits");
            }
            db.Add(account with { Id = null});
            await db.SaveChangesAsync();
            return account;
        }

        public async Task<Account?> Update(Account account)
        {
            using var db = new TasksDbContext();
            var record = await db.Accounts.FirstOrDefaultAsync(a => a.ChipId == account.ChipId);
            if (record == null)
            {
                return null;
            }
            record = record with { ChipId = account.ChipId, Name = account.Name, Role = account.Role };
            await db.SaveChangesAsync();
            return record;
        }
    }
}
