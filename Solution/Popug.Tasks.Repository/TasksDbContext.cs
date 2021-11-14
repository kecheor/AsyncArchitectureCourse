using Microsoft.EntityFrameworkCore;
using Popug.Tasks.Repository.Models;

namespace Popug.Tasks.Repository
{
    public class TasksDbContext : DbContext
    {
        public DbSet<Performer> Performer { set; get; }
        public DbSet<TaskData> Tasks { set; get; }
        public DbSet<TaskPerformerLog> PerformerLogs { set; get; }

        public TasksDbContext()
        {

        }

        public TasksDbContext(DbContextOptions<TasksDbContext> options) : base(options)
        {
        }


        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseNpgsql("Host=localhost;Port=5452;Database=popugs;Username=popug_admin;Password=popug123");
    }
}
