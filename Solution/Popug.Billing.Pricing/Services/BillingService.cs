using Popug.Billing.Pricing.Models;
using Popug.BillingRepository;
using Popug.Common.Monads;
using Popug.Common.Monads.Errors;

namespace Popug.Billing.Pricing.Services;

public class BillingService : IBillingService
{
    private readonly IPriceEstimationService _priceEstimationService;
    private readonly IBillingMessageGenerator _messageGenerator;
    private readonly IPriceRepository _repository;
    private readonly ILogger<IBillingService> _logger;

    public BillingService(IPriceRepository repository, 
        IPriceEstimationService priceEstimationService,
        IBillingMessageGenerator messageGenerator,
        ILogger<IBillingService> logger)
    {
        _messageGenerator = messageGenerator;
        _priceEstimationService = priceEstimationService;
        _repository = repository;
        _logger = logger;
    }

    public async Task<Either<BillingTransaction, Error>> PayTaskReward(string taskId, string performerId, CancellationToken cancellationToken)
    {
        var price = await GetPrices(taskId, cancellationToken);

        if (price.HasError)
        {
            return price.Error;
        }
        var reward = price.Result.CompleteReward;
        var transaction = new BillingTransaction(performerId, 0, reward, string.Empty);
        transaction = _messageGenerator.CreateTaskCompletePayoutMessage(transaction, GetTaskDescription(price.Result));
        return transaction;
    }

    public async Task<Either<BillingTransaction, Error>> WithdrawTaskPrice(string taskId, string performerId, CancellationToken cancellationToken)
    {
        var price = await GetPrices(taskId, cancellationToken);

        if (price.HasError)
        {
            return price.Error;
        }
        var withdrawal = price.Result.AssignPrice;
        var transaction = new BillingTransaction(performerId, withdrawal, 0, string.Empty);
        transaction = _messageGenerator.CreateTaskCompletePayoutMessage(transaction, GetTaskDescription(price.Result));
        return transaction;
    }

    private async Task<Either<TaskPrice, Error>> GetPrices(string taskId, CancellationToken cancellationToken)
    {
        var prices = await _repository.GetTaskPrices(taskId, cancellationToken);
        if (prices.HasError && prices.Error is PriceNotFoundError)
        {
            return await _priceEstimationService.EstimateTaskPrices(taskId, cancellationToken);
        }
        return prices;
    }

    private static string GetTaskDescription(TaskPrice price)
    {
        return string.IsNullOrEmpty(price.TaskDescription) 
            ? price.TaskId 
            : price.TaskDescription;
    }
}

