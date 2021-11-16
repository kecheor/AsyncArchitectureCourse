namespace Popug.Common.Monads.Errors;
public class MessageEventError : Error
{
    public string ErrorMessage { get; }
    public string Message { get; }

    public MessageEventError(string errorMessage, string message) : base(errorMessage)
    {
        Message = message;
    }
}