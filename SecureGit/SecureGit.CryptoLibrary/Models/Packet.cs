using System.Security;

namespace SecureGit.CryptoLibrary.Models
{
    public class Packet
    {
        // Header contains secret key wrapped by public key
        public string Header { get; set; }
        
        // Payload contains data wrapped by secret key
        public string Payload { get; set; }
    }
}