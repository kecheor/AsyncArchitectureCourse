namespace Popug.Billing.Pricing.Models
{
    public class BillingServicesConfiguration : IPriceEstimationConfiguration, ITransactionMessageConfiguration
    {
        public string BillingAmmoutPlaceholder { init; get; } = "{ammount}";
        public string TaskDescriptionPlaceholder { init; get; } = "{taskDesription}";
        public string TaskAssignWithdrawalTemplate { init; get; } = "Withdrawing ${ammount} for assigment of '{taskDesription}'";
        public string TaskCompletePayoutTemplate { init; get; } = "Rewarding with ${ammount} for completing of '{taskDesription}'";

        // Minimum price that can be withdrawn on assign
        public decimal MinPrice { init; get; } = 10;
        // Maximum price that can be withdrawn on assign
        public decimal MaxPrice { init; get; } = 20;
        // Minimum price that can be paid on completion
        public decimal MinReward { init; get; } = 20;
        // Maximum price that can be paid on completion
        public decimal MaxReward { init; get; } = 40;
    }
}
