using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace YouTubeClone.Shared.Extensions
{
    public static class RedisExtensions
    {
        public static async Task SetObjectAsync<T>(this IDistributedCache cache, string key, T value, TimeSpan? expiration = null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(60)
            };

            var jsonData = JsonSerializer.Serialize(value);
            await cache.SetStringAsync(key, jsonData, options);
        }

        public static async Task<T?> GetObjectAsync<T>(this IDistributedCache cache, string key)
        {
            var jsonData = await cache.GetStringAsync(key);
            return jsonData == null ? default : JsonSerializer.Deserialize<T>(jsonData);
        }
    }
}
