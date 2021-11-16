namespace Popug.Messages.Contracts.Values;
/// <summary>
/// Service record to store information about messages that could not be correctly handled during publish or consumption
/// </summary>
/// <param name="Source">Name of the producer or consumer where the error has occured</param>
/// <param name="ErrorMessage">Additional information about the error</param>
/// <param name="Exception">Information about occured exception if availible</param>
/// <param name="EventMessage">Raw broker message that caused the error</param>
public record MessageError(string Source, string ErrorMessage, string Exception, string EventMessage) : IEventValue
{
    public int Version => 1;
}
