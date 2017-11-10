namespace SecureGit.WebApi.Models
{
    public class PacketBox
    {
        // Header contains secret key
        public string Header { get; set; }
        // Payload contains data wrapped by secret key
        public string Payload { get; set; }
    }
}