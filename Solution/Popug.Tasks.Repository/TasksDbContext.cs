using Microsoft.EntityFrameworkCore;

namespace Popug.Tasks.Repository
{
    public class TasksDbContext : DbContext
    {
        public DbSet<Account> Accounts { set; get; }
        public DbSet<TaskData> Tasks { set; get; }

        public TasksDbContext(DbContextOptions<TasksDbContext> options) : base(options)
        {
        }

    }
}
