using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Popug.Common.Monads;
using Popug.Common.Monads.Errors;

namespace Popug.Accounts.Repository;

public class AccountRepository : IAccountRepository
{
    private readonly AccountsDbContext _dbContext;
    private readonly ILogger<AccountRepository> _logger;
    public AccountRepository(AccountsDbContext dbContext, ILogger<AccountRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Account>> GetAll(CancellationToken cancellationToken)
    {
        var records = await _dbContext.Accounts.ToArrayAsync(cancellationToken);
        _logger.LogTrace($"Found {records?.Length ?? 0} accounts");
        return records?.ToArray() ?? Array.Empty<Account>();
    }

    public async Task<Either<Account, Error>> Add(Account account, CancellationToken cancellationToken)
    {
        var exising = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.ChipId == account.ChipId || a.BeakCurvature == account.BeakCurvature, cancellationToken);
        if (exising != null)
        {
            return new Error("Such popug already exisits");
        }
        _dbContext.Add(account with { Id = null });
        await _dbContext.SaveChangesAsync();
        _logger.LogTrace($"Created new account {account.Id} for {account.ChipId}:{account.Name} with {account.BeakCurvature} beak");
        return account;
    }

    public async Task<Either<Account, Error>> Update(Account account, CancellationToken cancellationToken)
    {
        if (!account.Id.HasValue)
        {
            return new Error("Could not find account to update. Id is null");
        }

        var record = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == account.Id, cancellationToken);
        if (record == null)
        {
            return new Error("Could not find account to update.");
        }
        record = record with { ChipId = account.ChipId, Name = account.Name, Role = account.Role, BeakCurvature = account.BeakCurvature };
        await _dbContext.SaveChangesAsync();
        _logger.LogTrace($"Update account {account.Id} for {account.ChipId}:{account.Name} with {account.BeakCurvature} beak");
        return record;
    }

    public async Task<Account?> Find(int curvature, CancellationToken cancellationToken)
    {
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.BeakCurvature == curvature, cancellationToken);
        _logger.LogTrace($"Found popug {account.ChipId}:{account.Name} with {curvature} beak");
        return account;
    }

    public async Task<Account?> Find(string chipId, CancellationToken cancellationToken)
    {
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.ChipId == chipId, cancellationToken);
        _logger.LogTrace($"Found popug {account.ChipId}:{account.Name} with {chipId}");
        return account;
    }
}
