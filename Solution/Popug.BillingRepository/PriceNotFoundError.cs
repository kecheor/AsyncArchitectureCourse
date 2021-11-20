using Popug.Common.Monads.Errors;

namespace Popug.BillingRepository;
public class PriceNotFoundError : Error
{
    public PriceNotFoundError(string message) : base(message)
    {
    }
}
    