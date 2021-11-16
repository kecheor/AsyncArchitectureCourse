namespace Popug.Common.Monads.Errors;
public class Error
{
    public string ErrorMessage { get; }

    public Error(string message)
    {
        ErrorMessage = message;
    }
}