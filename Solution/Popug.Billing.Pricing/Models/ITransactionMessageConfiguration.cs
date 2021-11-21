namespace Popug.Billing.Pricing.Models;

public interface ITransactionMessageConfiguration
{
    public string BillingAmmoutPlaceholder { get; }
    public string TaskDescriptionPlaceholder { get; }
    public string TaskAssignWithdrawalTemplate { get; }
    public string TaskCompletePayoutTemplate { get; }
}
