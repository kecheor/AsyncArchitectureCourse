namespace Popug.Messages.Contracts.Values.CUD.Billing.v1;
public record TaskPriceValue(string TaskId, decimal AssignPrice, decimal CompleteReward, DateTime EstimationTimestamp) : IEventValue
{
    public int Version => 1;
}
