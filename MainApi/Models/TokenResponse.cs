using System.Text.Json.Serialization;

namespace MainApi.Models;

public class TokenResponse
{
    [JsonPropertyName("token")] 
    public string Token { get; set; }
}