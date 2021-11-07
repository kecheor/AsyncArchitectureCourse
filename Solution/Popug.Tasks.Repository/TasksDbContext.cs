using Microsoft.EntityFrameworkCore;

namespace Popug.Tasks.Repository
{
    public class TasksDbContext : DbContext
    {
        private const string _CONNECTION_STRING = "Host=localhost;Port=5452;Database=popugs;Username=popug_admin;Password=popug123";
        public DbSet<Account> Accounts { set; get; }
        public DbSet<Task> Tasks { set; get; }


        public TasksDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseNpgsql(_CONNECTION_STRING);
    }
}
