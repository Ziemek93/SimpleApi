namespace AuthApi.Options;

public class M2MOptions
{
    public string ClientId { get; set; } = string.Empty; // unique identifier of the token issuer for api
    public string Audience { get; set; } = string.Empty; // Audience - for what api this token is valid
    public string ClientSecret { get; set; } = string.Empty; // secret key for signing the token
    public string AllowedScopes { get; set; }
    public int Expiration { get; set; } = 120; // expiration time in secs  
}