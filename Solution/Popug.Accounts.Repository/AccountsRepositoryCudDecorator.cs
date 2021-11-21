using Popug.Common.Monads;
using Popug.Common.Monads.Errors;
using Popug.Common.Services;
using Popug.Messages.Contracts.Events;
using Popug.Messages.Contracts.EventTypes.CUD;
using Popug.Messages.Contracts.Services;
using Popug.Messages.Contracts.Values.CUD.Popugs;

namespace Popug.Accounts.Repository;

public class AccountsRepositoryCudDecorator : IAccountRepository, IDisposable
{
    private readonly IAccountRepository _accountRepository;
    private readonly IProducer _producer;
    //TODO:From configuration
    private static string TOPIC = "popug-accounts-stream";

    public AccountsRepositoryCudDecorator(IAccountRepository accountRepository, IProducer producer)
    {
        _accountRepository = accountRepository;
        _producer = producer;
    }

    public async Task<Either<Account, Error>> Add(Account account, CancellationToken cancellationToken)
    {
        var result = await _accountRepository.Add(account, cancellationToken);
        if (result.HasError)
        {
            return result;
        }

        var popug = new PopugValue(account.ChipId, account.Name, account.Role);
        var message = new NewEventMessage<PopugValue>(TOPIC, CudEventType.Created, nameof(AccountsRepositoryCudDecorator), popug);
        await _producer.Produce(message, cancellationToken);
        return result;
    }

    public Task<Account?> Find(int curvature, CancellationToken cancellationToken)
    {
        return _accountRepository.Find(curvature, cancellationToken);
    }

    public async Task<Account?> Find(string chipId, CancellationToken cancellationToken)
    {
        return await _accountRepository.Find(chipId, cancellationToken);
    }

    public Task<IReadOnlyList<Account>> GetAll(CancellationToken cancellationToken)
    {
        return _accountRepository.GetAll(cancellationToken);
    }

    public async Task<Either<Account, Error>> Update(Account account, CancellationToken cancellationToken)
    {
        var result = await  _accountRepository.Update(account, cancellationToken);
        if (result.HasError)
        {
            return result;
        }

        var popug = new PopugValue(account.ChipId, account.Name, account.Role);
        var message = new NewEventMessage<PopugValue>(TOPIC, CudEventType.Updated, nameof(AccountsRepositoryCudDecorator), popug);
        await _producer.Produce(message, cancellationToken);
        return account;
    }

    public void Dispose()
    {
        _producer.Dispose();
    }
}