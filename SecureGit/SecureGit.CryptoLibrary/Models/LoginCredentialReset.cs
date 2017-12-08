using System.Security;

namespace SecureGit.CryptoLibrary.Models
{
    public class LoginCredentialReset
    {
        public string Username { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}