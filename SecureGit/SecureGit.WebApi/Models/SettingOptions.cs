namespace SecureGit.WebApi.Models
{
    public class SettingOptions
    {
        public string UserDBPath { get; set; }
        public string JwtPrivateKey { get; set; }
        public string JwtIssuer { get; set; }
        public string JwtAudience { get; set; }
    }
}