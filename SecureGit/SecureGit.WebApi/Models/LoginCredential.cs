namespace SecureGit.WebApi.Models
{
    public class LoginCredential
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string RsaPublicKey { get; set; }
    }
}