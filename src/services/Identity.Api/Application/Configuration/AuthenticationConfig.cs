namespace Identity.Api.Application.Configuration
{
    public class AuthenticationConfig
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public string Secret { get; set; }
        public int ExpiresInMinutes { get; set; }
    }
}