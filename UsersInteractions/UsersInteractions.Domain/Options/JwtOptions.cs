namespace UsersInteractions.Domain.Options;

public class JwtOptions
{
    public string Issuer { get; set; } = string.Empty; // unique identifier of the token issuer for api
    public string Audience { get; set; } = string.Empty; // Audience - for what api this token is valid
}
