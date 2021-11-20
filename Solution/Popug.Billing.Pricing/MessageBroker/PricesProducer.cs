using Popug.Messages.Contracts.Services;

namespace Popug.Billing.Pricing.MessageBroker;

public class PricesProducer : IDisposable
{
    private readonly IProducer _producer;
    private readonly ILogger<PricesProducer> _logger;
    //TODO:From configuration
    private static string CUD_TOPIC = "popug-tasks-prices-stream";
    private static string BE_TOPIC = "popug-tasks-billing";

    public PricesProducer(IProducer producer, ILogger<PricesProducer> logger)
    {
        _producer = producer;
        _logger = logger;
    }

    public void Dispose()
    {
        _producer.Dispose();
    }
}