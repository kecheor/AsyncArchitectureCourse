using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Popug.Common.Monads;

namespace Popug.Accounts.IdentityServer.Services
{
    public interface IAccountIdentityService
    {
        Task<Either<IdentityServerUser, string>> FindAccount(int curvature, CancellationToken cancellationToken);
        AuthenticationProperties AuthenticationProperties { get; }
    }
}