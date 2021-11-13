using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Popug.Accounts.Authentication
{
    public static class AuthenticationHelper
    {
        public static void AddOidcAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOidcAuthentication(configuration.Get<AuthenticationConfiguration>());
        }

        public static void AddOidcAuthentication(this IServiceCollection services, IConfiguration configuration, string section)
        {
            services.AddOidcAuthentication(configuration.GetSection(section).Get<AuthenticationConfiguration>());
        }

        public static void AddOidcAuthentication(this IServiceCollection services, AuthenticationConfiguration configuration)
        {
            services
                .AddAuthentication(options =>
                 {
                     options.DefaultScheme = configuration.AuthenticationScheme;
                     options.DefaultChallengeScheme = "oidc";
                     options.DefaultSignOutScheme = "oidc";
                 })
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = configuration.Authority;
                    options.ClientId = configuration.ClientId;
                    options.ClientSecret = configuration.ClientSecret;
                    options.ResponseType = "code";
                    foreach (var scope in configuration.Scopes)
                    {
                        options.Scope.Add(scope);
                    }
                    options.SaveTokens = true;
                })
                .AddCookie(configuration.AuthenticationScheme);
        }
    }
}