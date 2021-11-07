using Duende.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;

namespace Popug.Accounts.IdentityServer
{
    public interface IAccountService
    {
        Task<IdentityServerUser> FindAccount(int curvature);
        AuthenticationProperties AuthenticationProperties { get; }
    }
}