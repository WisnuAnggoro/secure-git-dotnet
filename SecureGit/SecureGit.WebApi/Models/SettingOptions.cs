namespace SecureGit.WebApi.Models
{
    public class SettingOptions
    {
        public string UserDBPath { get; set; }
        public string UserDBFileName { get; set; }
        public string JwtPrivateKey { get; set; }
        public string JwtIssuer { get; set; }
        public string JwtAudience { get; set; }
        public string RsaKeyDirectory { get; set; }
        public string RsaPrivateKeyName { get; set; }
        public string RsaPublicKeyName { get; set; }
    }
}