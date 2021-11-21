namespace Popug.Billing.Pricing.Models;

public interface IPriceEstimationConfiguration
{
    public decimal MinPrice { get; }
    // Maximum price that can be withdrawn on assign
    public decimal MaxPrice { get; }
    // Minimum price that can be paid on completion
    public decimal MinReward { get; }
    // Maximum price that can be paid on completion
    public decimal MaxReward { get; }
}
