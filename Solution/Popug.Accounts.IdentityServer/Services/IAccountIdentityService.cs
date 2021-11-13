using IdentityServer4;
using Microsoft.AspNetCore.Authentication;

namespace Popug.Accounts.IdentityServer
{
    public interface IAccountIdentityService
    {
        Task<IdentityServerUser> FindAccount(int curvature, CancellationToken cancellationToken);
        AuthenticationProperties AuthenticationProperties { get; }
    }
}