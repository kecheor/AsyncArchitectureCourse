
namespace Popug.Tasks.Repository
{
    public interface IAccountRepository
    {
        Task<Account?> Add(Account account, CancellationToken cancellationToken);
        Task<IReadOnlyList<Account>> GetAll(CancellationToken cancellationToken);
        Task<Account?> Update(Account account, CancellationToken cancellationToken);
    }
}