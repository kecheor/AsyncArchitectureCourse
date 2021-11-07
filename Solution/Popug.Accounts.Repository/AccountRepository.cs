using Microsoft.EntityFrameworkCore;

namespace Popug.Accounts.Repository
{

    public class AccountRepository : IAccountRepository
    {
        public async Task<IReadOnlyList<Account>> GetAll()
        {
            using var db = new AccountsDbContext();
            var records = await db.Accounts.ToArrayAsync();
            return records?.ToArray() ?? Array.Empty<Account>();
        }

        public async Task<Account?> Add(Account account)
        {
            using var db = new AccountsDbContext();
            var exising = await db.Accounts.FirstOrDefaultAsync(a => a.ChipId == account.ChipId || a.BeakCurvature == account.BeakCurvature);
            if (exising != null)
            {
                throw new ArgumentException("Such popug already exisits");
            }
            db.Add(account with { Id = null });
            await db.SaveChangesAsync();
            return account;
        }

        public async Task<Account?> Update(Account account)
        {
            if (!account.Id.HasValue)
            {
                return null;
            }

            using var db = new AccountsDbContext();
            var record = await db.Accounts.FirstOrDefaultAsync(a => a.Id == account.Id);
            if (record == null)
            {
                return null;
            }
            record = record with { ChipId = account.ChipId, Name = account.Name, Role = account.Role, BeakCurvature = account.BeakCurvature };
            await db.SaveChangesAsync();
            return record;
        }


        public async Task<Account?> Find(int curvature)
        {
            using var db = new AccountsDbContext();
            return await db.Accounts.FirstOrDefaultAsync(a => a.BeakCurvature == curvature);
        }
    }
}
