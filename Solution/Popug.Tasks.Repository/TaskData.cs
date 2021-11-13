namespace Popug.Tasks.Repository
{
    public record TaskData(int Id, string Number, DateTime CreateTime, DateTime UpdateTime, string Text, int Performer, TaskStatus Status);
}
