using Microsoft.EntityFrameworkCore;

namespace Popug.Accounts
{
    public class AccountsDbContext : DbContext
    {
        private const string _CONNECTION_STRING = "Host=localhost;Port=5442;Database=popugs;Username=popug_admin;Password=popug123";
        public DbSet<Account> Accounts { set; get; }


        public AccountsDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseNpgsql(_CONNECTION_STRING);
    }
}
