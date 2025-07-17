using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using MyWebApi.Logs;
using MyWebApi.Services.IService;
using StackExchange.Redis;
using System.Text.Json;

namespace MyWebApi.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IMemoryCache _memoryCache;
        public CacheService(IDistributedCache distributedCache, IMemoryCache memoryCache)
        {
            _distributedCache = distributedCache;
            _memoryCache = memoryCache;
        }

        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            try
            {
                // Check memory cache first
                if (_memoryCache.TryGetValue(key, out T cachedValue))
                {
                    return cachedValue;
                }

                // Check distributed cache
                var distributedValue = await _distributedCache.GetStringAsync(key);
                if (!string.IsNullOrEmpty(distributedValue))
                {
                    var deserializedValue = JsonSerializer.Deserialize<T>(distributedValue);

                    // Store in memory cache for faster access
                    _memoryCache.Set(key, deserializedValue, TimeSpan.FromMinutes(5));

                    return deserializedValue;
                }

                return null;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return null;
            }
        }

        public async Task RemoveAllByAsync(string pattern)
        {
            try
            {
                // Custom Redis implementation for pattern removal
                var redis = ConnectionMultiplexer.Connect("your-redis-connection");
                var database = redis.GetDatabase();
                var server = redis.GetServer("your-redis-server");

                var keys = server.Keys(pattern: pattern);
                foreach (var key in keys)
                {
                    await database.KeyDeleteAsync(key);
                }
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _distributedCache.RemoveAsync(key);
                _memoryCache.Remove(key);
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
        {
            try
            {
                var serializedValue = JsonSerializer.Serialize(value);
                var options = new DistributedCacheEntryOptions();

                if (expiration.HasValue)
                {
                    options.SetAbsoluteExpiration(expiration.Value);
                }
                else
                {
                    options.SetAbsoluteExpiration(TimeSpan.FromHours(1)); // Cho mặc định 1h
                }

                await _distributedCache.SetStringAsync(key, serializedValue, options);

                // Set cả memory cache
                _memoryCache.Set(key, value, expiration ?? TimeSpan.FromMinutes(5));
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
            }
        }
    }
}
