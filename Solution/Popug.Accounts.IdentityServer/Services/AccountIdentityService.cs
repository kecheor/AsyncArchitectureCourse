using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Popug.Accounts.Repository;
using Popug.Common.Monads;
using System.Security.Claims;

namespace Popug.Accounts.IdentityServer.Services
{
    public class AccountIdentityService : IAccountIdentityService
    {
        private readonly IAccountRepository _accountRepository;
        private static AuthenticationProperties _authenticationProperties;
        private readonly ILogger<AccountIdentityService> _logger;

        public AccountIdentityService(IAccountRepository accountRepository, ILogger<AccountIdentityService> logger)
        {
            _logger = logger;
            _accountRepository = accountRepository;
            _authenticationProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(5),
                AllowRefresh = true,
            };
        }

        public async Task<Either<IdentityServerUser, string>> FindAccount(int curvature, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.Find(curvature, cancellationToken);
            if (account == null)
            {
                var message = $"Could not log in accout with beak {curvature}";
                _logger.LogInformation(message);
                return message;
            }
            _logger.LogInformation($"Logging in accout with beak {curvature}: {account.ChipId}:{account.Name}");
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
