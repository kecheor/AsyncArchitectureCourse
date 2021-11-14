using Popug.Common.Services;
using Popug.Messages.Contracts.Events;
using Popug.Messages.Contracts.EventTypes.CUD;
using Popug.Messages.Contracts.Services;
namespace Popug.Accounts.Repository;

public class AccountsRepositoryCudDecorator : IAccountRepository, IDisposable
{
    private readonly IAccountRepository _accountRepository;
    private readonly IProducer _producer;
    private readonly IJsonSerializer _jsonSerializer;
    //TODO:From configuration
    private static string TOPIC = "popug-accounts-stream";

    public AccountsRepositoryCudDecorator(IAccountRepository accountRepository, IProducer producer, IJsonSerializer jsonSerializer)
    {
        _accountRepository = accountRepository;
        _producer = producer;
        _jsonSerializer = jsonSerializer;
    }

    public async Task<Account?> Add(Account account, CancellationToken cancellationToken)
    {
        var result = await _accountRepository.Add(account, cancellationToken);
        if (result == null)
            return result;
        var message = new NewEventMessage<Account>(TOPIC, CudEventType.Created, nameof(AccountsRepositoryCudDecorator), account);
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

    public Task<Account?> Update(Account account, CancellationToken cancellationToken)
    {
        var message = new NewEventMessage<Account>(TOPIC, CudEventType.Updated, nameof(AccountsRepositoryCudDecorator), account);
        _producer.Produce(message, cancellationToken);
        return Task.FromResult((Account?)account);
    }

    private string Serialize(Account account)
    {
        return _jsonSerializer.Serialize(account);
    }

    public void Dispose()
    {
        _producer.Dispose();
    }
}