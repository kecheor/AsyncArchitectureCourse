using Popug.Billing.Pricing.Models;
using Popug.Common.Monads;
using Popug.Common.Monads.Errors;

namespace Popug.Billing.Pricing.Services;
public interface IBillingService
{
    Task<Either<BillingTransaction, Error>> WithdrawTaskPrice(string taskId, string performerId, CancellationToken cancellationToken);

    Task<Either<BillingTransaction, Error>> PayTaskReward(string taskId, string performerId, CancellationToken cancellationToken);
}
