namespace AuthApi.Options;

public class JwtOptions
{
    public string PrivateKey { get; set; } = string.Empty; // secret key for signing the token
    public string Issuer { get; set; } = string.Empty; // unique identifier of the token issuer for api
    public string Audience { get; set; } = string.Empty; // Audience - for what api this token is valid
    public int Expiration { get; set; } = 120; // expiration time in secs  
}
