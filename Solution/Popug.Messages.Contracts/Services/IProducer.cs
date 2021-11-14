using Popug.Common.Monads;
using Popug.Messages.Contracts.Events;

namespace Popug.Messages.Contracts.Services;
public interface IProducer : IDisposable
{
    Task<Either<None, Error>> Produce(string topic, EventMetadata metadata, string value, CancellationToken cancellationToken);
}
