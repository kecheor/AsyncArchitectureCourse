using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Popug.Accounts.IdentityServer.Services;

namespace Popug.Accounts.IdentityServer.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountIdentityService _accountService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAccountIdentityService accountService, ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        public IActionResult Login(string? returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string returnUrl, string curvature, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Login request for popug with {curvature}");
            if (!int.TryParse(curvature, out var c))
            {
                return View();
            }
            var account = await _accountService.FindAccount(c, cancellationToken);
            if(account.HasError)
            {
                return View(account.Error);
            }
            await HttpContext.SignInAsync(account.Result, _accountService.AuthenticationProperties);
            return Redirect(returnUrl);
        }

        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Logout request");
            if (User?.Identity?.IsAuthenticated == true)
            {
                await HttpContext.SignOutAsync();
                return Ok();
            }
            return StatusCode(StatusCodes.Status401Unauthorized);
        }

        public IActionResult Error(int? code)
        {
            return View();
        }
    }
}
