using IdentityServer4.Models;

namespace Popug.Accounts.IdentityServer.Configuration
{
    public class IdentityServerConfiguration
    {
        public static IEnumerable<Client> MapClients(IConfigurationSection configuration)
        {
            var clientConfigs = configuration.Get<ClientConfiguration[]>();
            return clientConfigs.Select(c => new Client
            {
                ClientId = c.ClientId,
                ClientSecrets = new[] { new Secret(c.ClientSecret.Sha256()) },
                AlwaysIncludeUserClaimsInIdToken = true,
                AllowedGrantTypes = GrantTypes.Code,
                RedirectUris = c.RedirectUris.Select(u => c.BaseUrl + u).ToArray(),
                PostLogoutRedirectUris = c.PostLogoutRedirectUris.Select(u => c.BaseUrl + u).ToArray(),
                AllowedScopes = c.AllowedScopes
            });
        }

        public static IEnumerable<ApiScope> MapScopes(IConfigurationSection configuration)
        {
            var scopes = configuration.Get<Dictionary<string, string>>();
            return scopes.Select(c => new ApiScope(c.Key, c.Value));
        }

        public static IEnumerable<IdentityResource> MapResources(IConfigurationSection configuration)
        {
            var resources = configuration.Get<string[]>();
            foreach (var item in resources)
            {
                yield return item.ToLowerInvariant() switch
                {
                    "openid" => new IdentityResources.OpenId(),
                    "profile" => new IdentityResources.Profile(),
                    "role" => new IdentityResource("role", new[] { "role" }),
                    _ => throw new ArgumentOutOfRangeException($"Unsupported Identity Resource {item}")
                };
            }

        }
    }
}
