namespace Popug.BillingRepository;
public record TaskPrice(string TaskId, string TaskDescription, decimal AssignPrice, decimal CompleteReward, DateTime EstimationTimestamp);
