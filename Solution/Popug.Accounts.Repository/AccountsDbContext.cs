using Microsoft.EntityFrameworkCore;
namespace Popug.Accounts;
public class AccountsDbContext : DbContext
{
    public DbSet<Account> Accounts { set; get; }

    public AccountsDbContext(DbContextOptions<AccountsDbContext> options) : base(options)
    {
    }
}
