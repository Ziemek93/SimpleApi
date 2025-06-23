using StackExchange.Redis;

namespace UsersInteractions.Application.Abstractions;

public interface ICacheService
{
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
    Task<T?> GetAsync<T>(string key);
    Task RemoveAsync(string key);
    Task<RedisValue[]> ListRangeAsync(string key);
    Task ListRightPushAsync(string key, string value);
    Task ListRightPushAsync(string key, RedisValue[] value);
    Task ListTrimAsync(string key, int maxMessagesToCache, int valuesToRemove);
}