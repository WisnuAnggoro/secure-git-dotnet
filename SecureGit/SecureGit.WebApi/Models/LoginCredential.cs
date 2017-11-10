using SecureGit.RsaLibrary.Models;

namespace SecureGit.WebApi.Models
{
    public class LoginCredential
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public RsaPublicKey RsaPublicKey { get; set; }
    }
}