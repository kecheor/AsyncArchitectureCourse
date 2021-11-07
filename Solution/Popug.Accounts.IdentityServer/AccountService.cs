using Duende.IdentityServer;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Popug.Accounts.IdentityServer
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private static AuthenticationProperties _authenticationProperties;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
            _authenticationProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(5),
                AllowRefresh = true,
            };
        }



        public async Task<IdentityServerUser?> FindAccount(int curvature)
        {
            var account = await _accountRepository.Find(curvature);
            if (account == null)
            {
                return null;
            }

            var isuser = new IdentityServerUser(account.ChipId)
            {
                DisplayName = account.Name,
            };

            return isuser;
        }
        public AuthenticationProperties AuthenticationProperties => _authenticationProperties;
    }
}
