using Flurl.Http;
using Flurl.Http.Configuration;
using MainApi.Interfaces;
using MainApi.Models.Auth;
using MainApi.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace MainApi.Services;

public class M2MTokenService : IM2MTokenService
 {
     private readonly IMemoryCache _cache;
     private readonly ILogger<M2MTokenService> _logger;
     private readonly M2MOptions _m2MOptions;
     private readonly AuthApiOptions _authApiOptions;
     private readonly IFlurlClient _flurlClient;
     private const string M2MTokenCacheKey = "m2m_token";
 
     public M2MTokenService(IOptions<M2MOptions> m2mOptions, IOptions<AuthApiOptions> authOptions, IFlurlClientCache flurlClientCache, IMemoryCache cache, ILogger<M2MTokenService> logger)
     {
         _m2MOptions = m2mOptions.Value;
         _authApiOptions = authOptions.Value;
         _cache = cache;
         _logger = logger;
         _flurlClient = flurlClientCache.GetOrAdd(_authApiOptions.Base, _authApiOptions.Base);
     }
 
     public async Task<string> GetTokenAsync(CancellationToken ct = default)
     {
         var path = $"{_authApiOptions.Base}{_authApiOptions.Paths.TokenM2M}";
         
         if (_cache.TryGetValue(M2MTokenCacheKey, out string? cachedToken) && !string.IsNullOrEmpty(cachedToken))
         {
             _logger.LogDebug("M2M token retrieved from cache.");
             return cachedToken;
         }
 
         _logger.LogInformation("Requesting new M2M token from auth server.");

         var client = new
         {
             ClientId = _m2MOptions.ClientId,
             ClientSecret = _m2MOptions.ClientSecret
         };

         var request = await _flurlClient
             .Request(path)
             .PostJsonAsync(client, cancellationToken: ct);
             var token = await request.GetStringAsync();
             
         if (string.IsNullOrWhiteSpace(token))//?.AccessToken))
         {
             _logger.LogCritical("Failed to retrieve M2M token.");
             throw new InvalidOperationException("Failed to retrieve M2M token.");
         }
         
         // save token to cache with time to live equals 1 minute
         var cacheEntryOptions = new MemoryCacheEntryOptions()
             .SetAbsoluteExpiration(TimeSpan.FromMinutes(1));

         _cache.Set(M2MTokenCacheKey, token, cacheEntryOptions);//response.AccessToken, cacheEntryOptions);
         _logger.LogInformation("New M2M token cached successfully.");
 
         return token;
     }
 }