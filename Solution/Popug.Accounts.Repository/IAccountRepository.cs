using Popug.Common.Monads;
using Popug.Common.Monads.Errors;

namespace Popug.Accounts.Repository;

public interface IAccountRepository
{
    Task<Either<Account, Error>> Add(Account account, CancellationToken cancellationToken);
    Task<Account?> Find(int curvature, CancellationToken cancellationToken);
    Task<Account?> Find(string chipId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Account>> GetAll(CancellationToken cancellationToken);
    Task<Either<Account, Error>> Update(Account account, CancellationToken cancellationToken);
}
