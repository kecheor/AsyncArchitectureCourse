
namespace Popug.Tasks.Repository
{
    public interface IAccountRepository
    {
        Task<Account?> Add(Account account);
        Task<IReadOnlyList<Account>> GetAll();
        Task<Account?> Update(Account account);
    }
}