namespace SecureGit.WebApi.Models
{
    public class UserDatabase
    {
        public string user { get; set; }
        public string pass { get; set; }
        public string email { get; set; }
        public UserRole role { get; set; }
    }
}