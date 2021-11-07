using Duende.IdentityServer;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Popug.Accounts;

namespace Popug.Accounts.IdentityServer.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
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
        public async Task<IActionResult> Login(string returnUrl, string curvature)
        {
            if (!int.TryParse(curvature, out var c))
            {
                return View();
            }

            var isuser = await _accountService.FindAccount(c);
            if (isuser == null)
            {
                return View();
            }


            await HttpContext.SignInAsync(isuser, _accountService.AuthenticationProperties);
            return Redirect(returnUrl);
        }

        public async Task<IActionResult> Logout()
        {
            if (User?.Identity.IsAuthenticated == true)
            {
                await HttpContext.SignOutAsync();
                return Ok();
            }
            return StatusCode(StatusCodes.Status401Unauthorized);
        }
    }
}
