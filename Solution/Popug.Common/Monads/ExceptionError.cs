namespace Popug.Common.Monads;
public class ExceptionError : Error
{
    public Exception Exception { get; }

    public ExceptionError(Exception ex) : base(ex.Message)
    {
        Exception = ex;
    }
}