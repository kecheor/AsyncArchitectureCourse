using Microsoft.EntityFrameworkCore;
namespace Popug.Tasks.Repository.Models;

[Keyless]
public class TaskPerformerLog
{
    public int TaskId { set; get; }
    public int PerformerId { set; get; }
    public DateTime Assigned { set; get; }
}
