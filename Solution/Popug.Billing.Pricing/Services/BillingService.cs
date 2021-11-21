using Popug.Billing.Pricing.Models;
using Popug.BillingRepository;
using Popug.Common.Monads;
using Popug.Common.Monads.Errors;

namespace Popug.Billing.Pricing.Services;

public class BillingService : IBillingService
{
    private readonly IPriceEstimationService _priceEstimationService;
    private readonly ITransactionMessageGenerator _messageGenerator;
    private readonly IPriceRepository _repository;
    private readonly ILogger<IBillingService> _logger;

    public BillingService(IPriceRepository repository, 
        IPriceEstimationService priceEstimationService,
        ITransactionMessageGenerator messageGenerator,
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
        transaction = _messageGenerator.CreateTaskCompletePayoutMessage(transaction, price.Result.TaskDescription);
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
        transaction = _messageGenerator.CreateTaskCompletePayoutMessage(transaction, price.Result.TaskDescription);
        return transaction;
    }

    private async Task<Either<TaskPrice, Error>> GetPrices(string taskId, CancellationToken cancellationToken)
    {
        var prices = await _repository.GetTaskPrices(taskId, cancellationToken);
        if (prices.HasError && prices.Error is PriceNotFoundError)
        {
            return await EstimatePrices(taskId, cancellationToken);
        }
        return prices;
    }

    private async Task<Either<TaskPrice, Error>> EstimatePrices(string taskId, CancellationToken cancellationToken)
    {
        var estimation = await _priceEstimationService.EstimateTaskPrices(taskId, cancellationToken);
        if (estimation.HasError)
        {
            return estimation.Error;
        }
        var prices = estimation.Result;
        // !!! Important. !!!
        // When prices have to be estimated from billing service we do not know the task description. 
        // Therefore public task id will be used and also mentioned in transaction message
        // TODO: Find acceptable solution with management 
        var taskPrice = new TaskPrice(prices.TaskId, prices.TaskId, prices.AssignPrice, prices.CompleteReward, prices.EstimationTimestamp);
        await _repository.SetTaskPrices(taskPrice, cancellationToken);
        return taskPrice;
    }
}

