using Microsoft.Extensions.Logging;
using Popug.BillingRepository;
using Popug.Common.Monads;
using Popug.Common.Monads.Errors;
using Popug.Common.Services;
using StackExchange.Redis;

namespace Popug.Billing.Repository.Redis;
public class PriceRepository : IPriceRepository
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly ILogger<IPriceRepository> _logger;

    public PriceRepository(IConnectionMultiplexer redis, IJsonSerializer jsonSerializer, ILogger<IPriceRepository> logger)
    {
        _redis = redis;
        _jsonSerializer = jsonSerializer;
        _logger = logger;
    }

    public async Task<Either<TaskPrice, Error>> GetTaskPrices(string taskId, CancellationToken cancellationToken)
    {
        IDatabase db = _redis.GetDatabase();
        string value = await db.StringGetAsync(taskId);
        if(string.IsNullOrEmpty(value))
        {
            return new PriceNotFoundError($"Price for task {taskId} not found");
        }
        try
        {
            var price = _jsonSerializer.Deserialize<TaskPrice>(value);
            _logger.LogTrace($"Found task {taskId}: price = {price.AssignPrice}, reward = {price.CompleteReward}");
            return price;
        }
        catch (Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    public async Task<Either<TaskPrice, Error>> SetTaskPrices(TaskPrice prices, CancellationToken cancellationToken)
    {
        try
        {
            IDatabase db = _redis.GetDatabase();
            
            string exising = await db.StringGetAsync(prices.TaskId);
            if(!string.IsNullOrEmpty(exising))
            {
                var oldPrice = _jsonSerializer.Deserialize<TaskPrice>(exising);
                prices = MergeRecords(oldPrice, prices);
            }
            
            var value = _jsonSerializer.Serialize(prices);
            await db.StringSetAsync(prices.TaskId, value);
            return prices;
        }
        catch(Exception ex)
        {
            return new ExceptionError(ex);
        }
    }

    private TaskPrice MergeRecords(TaskPrice exisingPrice, TaskPrice newPrice)
    {
        return newPrice with { AssignPrice = exisingPrice.AssignPrice, CompleteReward = newPrice.CompleteReward };
    }
}
