namespace SecureGit.WebApi.Models
{
    public class SettingOptions
    {
        public string UserDBPath { get; set; }
        public string UserDBFileName { get; set; }
        public string GitSourcePath { get; set; }
        public string ProjectListFileName { get; set; }
        public string JwtPrivateKey { get; set; }
        public string JwtIssuer { get; set; }
        public string JwtAudience { get; set; }
        public string RsaKeyDirectory { get; set; }
        public string RsaPrivateKeyName { get; set; }
        public string RsaPublicKeyName { get; set; }
        public string ConnectionString { get; set; }
    }
}