using Popug.Common.Monads;
using Popug.Messages.Contracts.Events;
using Error = Popug.Common.Monads.Error;

namespace Popug.Messages.Contracts.Services;
public interface IConsumer : IDisposable
{
    Either<ConsumedEvent, Error> Consume(int millisecondsTimeout);

    Either<ConsumedEvent, Error> Consume(CancellationToken cancellationToken = default(CancellationToken));

    Either<ConsumedEvent, Error> Consume(TimeSpan timeout);

    void Subscribe(IEnumerable<string> topics);

    void Subscribe(string topic);

    void Unsubscribe();

    void Close();
}
