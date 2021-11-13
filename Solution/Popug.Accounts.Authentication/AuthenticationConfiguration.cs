namespace Popug.Accounts.Authentication
{
    public class AuthenticationConfiguration
    {
        public string Authority { get; init; }
        public string ClientId { get; init; }
        public string ClientSecret { get; init; }
        public string[] Scopes { get; init; }
        public string AuthenticationScheme { get; init; } = "Cookies";
    }
}
