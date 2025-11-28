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
        TimeSpan? absoluteExpirationRelativeToNow = null,
        ILogger? logger = null)
    {
        // Try to get from cache first
        try
        {
            logger?.LogTrace("Reading Redis key {Key}", key);
            var cachedValue = await cache.GetStringAsync(key);
            if (cachedValue != null)
            {
                var deserialized = JsonSerializer.Deserialize<T>(cachedValue);
                if (deserialized != null)
                {
                    logger?.LogDebug("Cache hit for {Key}", key);
                    return deserialized;
                }
            }
            logger?.LogDebug("Cache null for {Key}", key);
        }
        catch (Exception ex)
        {
            // If cache read fails, continue to database query (best effort)
            logger?.LogError(ex, "Error reading Redis key {Key}", key);
        }

        // Cache miss or cache read failed - get from database
        var item = await getItem();
        
        if (item == null) return item;
        // Try to store in cache (best effort - don't fail if this doesn't work)
        try
        {
            logger?.LogTrace("Writing Redis key {Key}", key);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow ?? TimeSpan.FromHours(24)
            };

            var json = JsonSerializer.Serialize(item);
            await cache.SetStringAsync(key, json, options);
            logger?.LogDebug("Successfully wrote Redis key {Key}", key);
        }
        catch (Exception ex)
        {
            // If cache write fails, we still return the item we got from database
            // This is fine - next request will query database again
            logger?.LogError(ex, "Failed writing Redis key {Key}", key);
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

