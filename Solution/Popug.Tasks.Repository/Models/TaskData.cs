using Popug.Common;

namespace Popug.Tasks.Repository.Models;
public class TaskData
{
    public int? Id { get; set; }
    public string TaskPublicId { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public string Text { get; set; }
    public Performer Performer { get; set; }
    public TaskState Status { get; set; }
}
