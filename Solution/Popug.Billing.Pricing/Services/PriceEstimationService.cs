using Popug.BillingRepository;
using Popug.Common.Monads;
using Popug.Common.Monads.Errors;

namespace Popug.Billing.Pricing.Services;
public class PriceEstimationService : IPriceEstimationService
{
    private readonly Random _random = new Random();
    private readonly PriceEstimationConfiguration _configuration;

    public PriceEstimationService(PriceEstimationConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<Either<TaskPrice, Error>> EstimateTaskPrices(string taskId, CancellationToken cancellationToken)
    {
        try
        {
            var price = EstimatePrice(_configuration.MinPrice, _configuration.MaxPrice);
            var reward = EstimatePrice(_configuration.MinReward, _configuration.MaxReward);
            return Task.FromResult(Either<TaskPrice, Error>.Success(new TaskPrice(taskId, price, reward, DateTime.UtcNow)));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Either<TaskPrice, Error>.Failure(new ExceptionError($"Could not estimate task {taskId}", ex)));
        }
    }

    private decimal EstimatePrice(decimal minValue, decimal maxValue)
    {
        var maxRand = (int)Math.Floor((minValue + maxValue) * 100);
        var value = _random.Next(maxRand);
        var result = (decimal)Math.Floor(value / 100d);
        return result;
    }
}
