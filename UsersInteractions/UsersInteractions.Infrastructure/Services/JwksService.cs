using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UsersInteractions.Domain.Options;

namespace UsersInteractions.Infrastructure.Services;

public class JwksService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<JwksService> _logger;
    private readonly AuthApiOptions _authApiOptions;
    private readonly IFlurlClient _flurlClient;

    private const string JwksCacheKey = "jwks_keys";
    
    public JwksService(IFlurlClientCache flurlClient, IOptions<AuthApiOptions> authApiOptions, IMemoryCache cache, ILogger<JwksService> logger)
    {
        _authApiOptions = authApiOptions.Value;
        _flurlClient = flurlClient.GetOrAdd(_authApiOptions.Base, _authApiOptions.Base);        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<SecurityKey>> GetSigningKeysAsync(CancellationToken ct = default)
    {
        if (_cache.TryGetValue(JwksCacheKey, out IEnumerable<SecurityKey> cachedKeys))
        {
            _logger.LogInformation($"{nameof(JwksService)}: public key is already loaded");
            return cachedKeys!;
        }

        _logger.LogWarning("Loading key from AuthApi.");

        var path = $"{_authApiOptions.Base}{_authApiOptions.Paths.PublicKey}";
            
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