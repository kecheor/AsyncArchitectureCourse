
namespace Popug.Accounts
{
    public interface IAccountRepository
    {
        Task<Account?> Add(Account account);
        Task<Account?> Find(int curvature);
        Task<IReadOnlyList<Account>> GetAll();
        Task<Account?> Update(Account account);
    }
}