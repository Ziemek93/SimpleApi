namespace MainApi.Options;

public class M2MOptions
{
    public string ClientId { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string AllowedScopes { get; set; } = string.Empty;
}
