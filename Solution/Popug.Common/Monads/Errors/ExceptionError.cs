namespace Popug.Common.Monads.Errors;
public class ExceptionError : Error
{
    public Exception Exception { get; }

    public ExceptionError(Exception ex) : base(ex.Message)
    {
        Exception = ex;
    }

    public ExceptionError(string message, Exception ex) : base(message)
    {
        Exception = ex;
    }
}