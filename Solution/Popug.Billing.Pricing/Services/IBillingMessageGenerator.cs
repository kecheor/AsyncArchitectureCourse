using Popug.Billing.Pricing.Models;

namespace Popug.Billing.Pricing.Services;
public interface IBillingMessageGenerator
{
    public BillingTransaction CreateTaskAssignWithdrawalMessage(BillingTransaction transaction, string taskDescription);
    public BillingTransaction CreateTaskCompletePayoutMessage(BillingTransaction transaction, string taskDescription);
}
