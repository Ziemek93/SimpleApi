using System.Text.Json;
using StackExchange.Redis;
using UsersInteractions.Application.Abstractions;

namespace UsersInteractions.Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _database;

    public RedisCacheService(IConnectionMultiplexer connection)
    {
        _database = connection.GetDatabase();
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var json = JsonSerializer.Serialize(value);
        await _database.StringSetAsync(key, json, expiration);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _database.StringGetAsync(key);
        return value.HasValue ? JsonSerializer.Deserialize<T>(value!) : default;
    }

    public async Task RemoveAsync(string key)
    {
        await _database.KeyDeleteAsync(key);
    }

    public async Task<RedisValue[]> ListRangeAsync(string key)
    {
        return await _database.ListRangeAsync(key);
    }

    public async Task ListRightPushAsync(string key, string value)
    {
        await _database.ListRightPushAsync(key, value);
    }

    public async Task ListRightPushAsync(string key, RedisValue[] value)
    {
        await _database.ListRightPushAsync(key, value);
    }

    public async Task ListTrimAsync(string key, int maxMessagesToCache, int valuestoRemove)
    {
        await _database.ListTrimAsync(key, -maxMessagesToCache, -valuestoRemove);
    }
}