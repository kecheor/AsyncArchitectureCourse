using IdentityServer4.Models;
using IdentityServer4.Services;
using Popug.Accounts.Repository;
using System.Security.Claims;

namespace Popug.Accounts.IdentityServer.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IAccountRepository _accountRepository;

        public ProfileService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var account = await _accountRepository.Find(GetSubjectChipId(context.Subject), default(CancellationToken));
            if(account == null)
            {
                return;
            }

            var claims = new List<Claim>
            {
                new Claim("displayName", account.Name),
                new Claim("role", account.Role.ToString())
            };

            context.IssuedClaims.AddRange(claims);
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
        }

        private static string GetSubjectChipId(ClaimsPrincipal subject)
        {
            return subject.Claims.First(c => c.Type == "sub").Value;
        }
    }
}
