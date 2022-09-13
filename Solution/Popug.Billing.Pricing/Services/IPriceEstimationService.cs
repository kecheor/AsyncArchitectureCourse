using Popug.Billing.Pricing.Models;
using Popug.BillingRepository;
using Popug.Common.Monads;
using Popug.Common.Monads.Errors;

namespace Popug.Billing.Pricing.Services;
public interface IPriceEstimationService
{
    Task<Either<TaskPriceEstimation, Error>> EstimateTaskPrices(string taskId, CancellationToken cancellationToken);
}
