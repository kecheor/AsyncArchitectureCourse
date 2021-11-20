namespace Popug.Billing.Pricing.MessageBroker;

public interface IPricesProducer
{
    Task ChangePerformerBallance(string performerId, decimal withdrawal, decimal deposit, string description);
}
