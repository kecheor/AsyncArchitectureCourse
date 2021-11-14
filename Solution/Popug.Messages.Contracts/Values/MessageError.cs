namespace Popug.Messages.Contracts.Values;
public record MessageError(string Source, string ErrorMessage, string Exception, string EventMessage) : IEventValue
{
    public int Version => 1;
}
