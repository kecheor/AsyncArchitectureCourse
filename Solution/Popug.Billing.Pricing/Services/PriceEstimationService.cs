using Popug.Billing.Pricing.Models;
using Popug.BillingRepository;
using Popug.Common.Monads;
using Popug.Common.Monads.Errors;

namespace Popug.Billing.Pricing.Services;
public class PriceEstimationService : IPriceEstimationService
{
    private readonly Random _random = new Random();
    private readonly IPriceEstimationConfiguration _configuration;

    public PriceEstimationService(IPriceEstimationConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<Either<TaskPriceEstimation, Error>> EstimateTaskPrices(string taskId, CancellationToken cancellationToken)
    {
        try
        {
            var price = EstimatePrice(_configuration.MinPrice, _configuration.MaxPrice);
            var reward = EstimatePrice(_configuration.MinReward, _configuration.MaxReward);
            return Task.FromResult(Either<TaskPriceEstimation, Error>.Success(new TaskPriceEstimation(taskId, price, reward, DateTime.UtcNow)));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Either<TaskPriceEstimation, Error>.Failure(new ExceptionError($"Could not estimate task {taskId}", ex)));
        }
    }

    private decimal EstimatePrice(decimal minValue, decimal maxValue)
    {
        var randDelta = (maxValue - minValue) * 100;
        var value = _random.Next((int)randDelta);
        var result = (decimal)Math.Floor(value / 100d) + minValue;
        return result;
    }
}
