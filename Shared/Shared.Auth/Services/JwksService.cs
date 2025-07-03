using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Auth.Options;

namespace Shared.Auth.Services;

public class JwksService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<JwksService> _logger;
    private readonly PublicKeyOptions _publicKeyOptions;
    private readonly IFlurlClient _flurlClient;

    private const string JwksCacheKey = "jwks_keys";
    
    public JwksService(IFlurlClientCache flurlClient, IOptions<PublicKeyOptions> publicKeyOptions, IMemoryCache cache, ILogger<JwksService> logger)
    {
        _publicKeyOptions = publicKeyOptions.Value;
        _flurlClient = flurlClient.GetOrAdd(_publicKeyOptions.Base);        
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<SecurityKey>> GetSigningKeysAsync(CancellationToken ct = default)
    {
        if (_cache.TryGetValue(JwksCacheKey, out IList<JsonWebKey> cachedKeys))
        {
            _logger.LogInformation($"{nameof(JwksService)}: public key is already loaded");
            return cachedKeys!;
        }

        _logger.LogWarning("Loading key from AuthApi.");

        var path = $"{_publicKeyOptions.Base}{_publicKeyOptions.Key}";
            
        // keys
        var jwks = await _flurlClient.Request(path)
            .WithHeader("Accept", "application/json")
            .WithHeader("Content-Type", "application/json")
            .WithTimeout(30)
            .GetJsonAsync<JsonWebKeySet>(cancellationToken: ct);

        if (jwks == null || jwks.Keys.Count == 0)
        {
            _logger.LogCritical($"{nameof(JwksService)}: Cannot access AuthApi or key not found!");
            throw new InvalidOperationException($"{nameof(JwksService)}: Cannot access AuthApi or key not found!");        }
        
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromHours(24));
        
        _cache.Set(JwksCacheKey, jwks.Keys, cacheEntryOptions);
        
        return jwks.Keys;
    }
}