namespace Popug.Common;
public interface IMessageErrorLogger
{
    Task LogError(string source, string errorMessage, string? eventMessage, Exception? exception, CancellationToken cancellationToken);
}