using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Popug.Accounts.Repository;

namespace Popug.Accounts.Management;

[Route("api")]
[ApiController]
public class AccountsController : ControllerBase
{
    IAccountRepository _repository;

    public AccountsController(IHttpContextAccessor contextAccessor, IAccountRepository repository)
    {
        _repository = repository;
    }

    [Route("all")]
    public async Task<IActionResult> All(CancellationToken cancellationToken)
    {
        return Ok(await _repository.GetAll(cancellationToken));
    }

    [Route("add")]
    public async Task<IActionResult> Add(Account account, CancellationToken cancellationToken)
    {
        return Ok(await _repository.Add(account, cancellationToken));
    }

    [Route("update")]
    public async Task<IActionResult> Update(Account account, CancellationToken cancellationToken)
    {
        return Ok(await _repository.Update(account, cancellationToken));
    }
}
