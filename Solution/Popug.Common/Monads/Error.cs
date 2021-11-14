namespace Popug.Common.Monads;
public class Error
{
    public string ErrorMessage { get; }

    public Error(string message)
    {
        ErrorMessage = message;
    }
}