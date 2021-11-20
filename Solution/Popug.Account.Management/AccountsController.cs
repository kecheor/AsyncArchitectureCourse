using Microsoft.AspNetCore.Mvc;
using Popug.Accounts.Repository;

namespace Popug.Accounts.Management;

[Route("api")]
[ApiController]
public class AccountsController : ControllerBase
{
    private readonly IAccountRepository _repository;
    private readonly ILogger<AccountsController> _logger;

    public AccountsController(IHttpContextAccessor contextAccessor, IAccountRepository repository, ILogger<AccountsController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [Route("all")]
    public async Task<IActionResult> All(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Returning list of all accounts");
        return Ok(await _repository.GetAll(cancellationToken));
    }

    [Route("add")]
    public async Task<IActionResult> Add(Account account, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Adding new account for {account.ChipId}:{account.Name} with {account.BeakCurvature} for role {account.Role}");
        return Ok(await _repository.Add(account, cancellationToken));
    }

    [Route("update")]
    public async Task<IActionResult> Update(Account account, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Updating account {account.Id} for {account.ChipId}:{account.Name} with {account.BeakCurvature} for role {account.Role}");
        return Ok(await _repository.Update(account, cancellationToken));
    }
}
