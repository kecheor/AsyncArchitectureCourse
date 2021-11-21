using Popug.Billing.Pricing.Models;
using System.Text;

namespace Popug.Billing.Pricing.Services
{
    public class TransactionMessageGenerator : ITransactionMessageGenerator
    {
        private readonly ITransactionMessageConfiguration _configuration;

        public TransactionMessageGenerator(ITransactionMessageConfiguration configuration)
        {
            _configuration = configuration;
        }

        public BillingTransaction CreateTaskAssignWithdrawalMessage(BillingTransaction transaction, string taskDescription)
        {
            var message = GenerateMessage(_configuration.TaskAssignWithdrawalTemplate, taskDescription, transaction.Withdrawal);
            return transaction with { Description = message };
        }

        public BillingTransaction CreateTaskCompletePayoutMessage(BillingTransaction transaction, string taskDescription)
        {
            var message = GenerateMessage(_configuration.TaskCompletePayoutTemplate, taskDescription, transaction.Deposit);
            return transaction with { Description = message };
        }

        private string GenerateMessage(string template, string taskDescription, decimal ammount)
        {
            var sb = new StringBuilder(template);
            sb.Replace(_configuration.BillingAmmoutPlaceholder, ammount.ToString());
            sb.Replace(_configuration.TaskDescriptionPlaceholder, taskDescription);
            return sb.ToString();
        }
    }
}
