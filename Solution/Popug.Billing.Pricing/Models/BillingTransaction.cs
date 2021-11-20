namespace Popug.Billing.Pricing.Models;
public record BillingTransaction(string PerformerId, decimal Withdrawal, decimal Deposit, string Description)
