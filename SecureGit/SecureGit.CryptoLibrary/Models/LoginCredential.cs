using System.Security;

namespace SecureGit.CryptoLibrary.Models
{
    public class LoginCredential
    {
        public string Username { get; set; }
        public SecureString Password { get; set; }
        public string RsaPublicKey { get; set; }
    }
}