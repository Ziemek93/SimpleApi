namespace UsersInteractions.Domain.Options;

public class AuthApiOptions
{
    public string Base { get; set; } = string.Empty;
    public Uris Paths { get; set; }

    public class Uris
    {
        public string Login { get; set; } = string.Empty;
        public string Register { get; set; } = string.Empty;
        public string Refresh { get; set; } = string.Empty;
        public string Logout { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
    }
}
