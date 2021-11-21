namespace Popug.Messages.Contracts.Values.BE.Billing.v1;

public record EmployeeBallanceTransation(string PerformerId, decimal Withdrawal, decimal Deposit, string Description) : IEventValue
{
    public int Version => 1;
}
