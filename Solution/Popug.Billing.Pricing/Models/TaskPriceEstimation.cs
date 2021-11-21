namespace Popug.Billing.Pricing.Models;

public record TaskPriceEstimation(string TaskId, decimal AssignPrice, decimal CompleteReward, DateTime EstimationTimestamp);
