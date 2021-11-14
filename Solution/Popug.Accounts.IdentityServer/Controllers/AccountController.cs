using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Popug.Accounts.IdentityServer.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountIdentityService _accountService;

        public AccountController(IAccountIdentityService accountService)
        {
            _accountService = accountService;
        }

        public IActionResult Login(string? returnUrl)
        {
            if(string.IsNullOrEmpty(returnUrl))
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string returnUrl, string curvature, CancellationToken cancellationToken)
        {
            if (!int.TryParse(curvature, out var c))
            {
                return View();
            }

            var isuser = await _accountService.FindAccount(c, cancellationToken);
            if (isuser == null)
            {
                return View();
            }


            await HttpContext.SignInAsync(isuser, _accountService.AuthenticationProperties);
            return Redirect(returnUrl);
        }

        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
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
