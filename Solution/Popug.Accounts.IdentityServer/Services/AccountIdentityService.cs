using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Popug.Accounts.Repository;
using System.Security.Claims;

namespace Popug.Accounts.IdentityServer
{
    public class AccountIdentityService : IAccountIdentityService
    {
        private readonly IAccountRepository _accountRepository;
        private static AuthenticationProperties _authenticationProperties;

        public AccountIdentityService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
            _authenticationProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(5),
                AllowRefresh = true,
            };
        }

        public async Task<IdentityServerUser?> FindAccount(int curvature, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.Find(curvature, cancellationToken);
            if (account == null)
            {
                return null;
            }

            var isuser = new IdentityServerUser(account.ChipId)
            {
                DisplayName = account.Name,
                AdditionalClaims = new[]
                {
                    new Claim(ClaimTypes.Name, account.Name),
                    new Claim(ClaimTypes.Role, account.Role.ToString())
                }
            };

            return isuser;
        }
        public AuthenticationProperties AuthenticationProperties => _authenticationProperties;
    }
}
