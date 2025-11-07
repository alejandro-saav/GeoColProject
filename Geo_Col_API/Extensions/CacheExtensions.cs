using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Geo_Col_API.Extensions;

public static class CacheExtensions
{
    /// <summary>
    /// Gets a cached value or executes the function and caches the result.
    /// Works for both single objects and collections (List, IEnumerable, etc.)
    /// </summary>
    public static async Task<T> GetOrSetAsync<T>(
        this IDistributedCache cache,
        string key,
        Func<Task<T>> getItem,
        TimeSpan? absoluteExpirationRelativeToNow = null)
    {
        // Try to get from cache first
        try
        {
            var cachedValue = await cache.GetStringAsync(key);
            if (cachedValue != null)
            {
                var deserialized = JsonSerializer.Deserialize<T>(cachedValue);
                if (deserialized != null)
                {
                    return deserialized;
                }
            }
        }
        catch (Exception)
        {
            // If cache read fails, continue to database query (best effort)
        }

        // Cache miss or cache read failed - get from database
        var item = await getItem();

        // Try to store in cache (best effort - don't fail if this doesn't work)
        try
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow ?? TimeSpan.FromHours(24)
            };

            var json = JsonSerializer.Serialize(item);
            await cache.SetStringAsync(key, json, options);
        }
        catch (Exception)
        {
            // If cache write fails, we still return the item we got from database
            // This is fine - next request will query database again
        }

        return item;
    }

    /// <summary>
    /// Invalidates a cache key (deletes it from Redis)
    /// </summary>
    public static async Task InvalidateAsync(this IDistributedCache cache, string key)
    {
        try
        {
            await cache.RemoveAsync(key);
        }
        catch (Exception)
        {
            // Silently fail - cache invalidation is not critical
        }
    }
}

