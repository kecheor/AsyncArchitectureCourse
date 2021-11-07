// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using System.Collections.Generic;

namespace Popug.Accounts.IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("accounts", "Account Management"),
                new ApiScope("tasks", "Tasks Management"),
                //new ApiScope("billing", "Billing"),
                //new ApiScope("analytics", "Analytics"),
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                 new Client
                {
                    ClientId = "accounts",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,
    
                    // where to redirect to after login
                    RedirectUris = { "https://localhost:7144/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "https://localhost:7144/signout-callback-oidc" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "accounts"
                    }
                },

                new Client
                {
                    ClientId = "bff",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,
    
                    // where to redirect to after login
                    RedirectUris = { "https://localhost:7204/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "https://localhost:7204/signout-callback-oidc" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "tasks",
                    }
                }
            };
    }
}
