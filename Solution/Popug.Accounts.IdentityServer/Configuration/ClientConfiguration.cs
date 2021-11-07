namespace Popug.Accounts.IdentityServer.Configuration
{
    public class ClientConfiguration
    {
        public string ClientId { init; get; } = "";
        public string ClientSecret { init; get; } = "";
        public string BaseUrl { init; get; } = "";
        public string[] RedirectUris { init; get; } = new string[0];
        public string[] PostLogoutRedirectUris { init; get; } = new string[0];
        public string[] AllowedScopes { init; get; } = new string[0];
    }
}