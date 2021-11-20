using Popug.Common.Monads;
using Popug.Common.Monads.Errors;

namespace Popug.BillingRepository
{
    public interface IPriceRepository
    {
        Task<Either<TaskPrice, Error>> GetTaskPrices(string taskId, CancellationToken cancellationToken);

        Task<Either<TaskPrice, Error>> SetTaskPrices(TaskPrice prices, CancellationToken cancellationToken);
    }
}
