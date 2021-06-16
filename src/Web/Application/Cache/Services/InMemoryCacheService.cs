using System;
using System.Threading.Tasks;
using EventManagement.Core.Utilities;
using EventManagement.Web.Application.Cache.Enumerations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace EventManagement.Web.Application.Cache.Services
{
    public interface IInMemoryCacheService
    {
        T Cache<T>(Func<T> addToCacheLoader, string key, int duration = CacheDuration.CacheLong,
            bool refreshObject = false);

        Task<T> CacheAsync<T>(Func<Task<T>> addToCacheLoader, string key,
            int duration = CacheDuration.CacheLong,
            bool refreshObject = false);
    }

    public class InMemoryCacheService : IInMemoryCacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<InMemoryCacheService> _logger;

        public InMemoryCacheService(IMemoryCache memoryCache,
            ILogger<InMemoryCacheService> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public T Cache<T>(Func<T> addToCacheLoader, string key, int duration = CacheDuration.CacheLong,
            bool refreshObject = false)
        {
            var cacheKey = GenerateKey<T>(key);
            try
            {
                if (refreshObject)
                {
                    _logger.LogInformation("object of {Type} with {Key} refreshed for {Duration}",
                        typeof(T), key, duration);
                    return InsertCacheObject(addToCacheLoader, cacheKey, duration);
                }

                if (!_memoryCache.TryGetValue(cacheKey, out T cachedObject))
                    return cachedObject == null
                        ? InsertCacheObject(addToCacheLoader, cacheKey, duration)
                        : cachedObject;

                _logger.LogInformation("object of {Type} with {Key} was fetched from cache",
                    typeof(T), key);
                return cachedObject;
            }
            catch (NullReferenceException)
            {
                // When T is a struct cachedObject cannot be null because structs are value types
                _logger.LogInformation("object of {Type} with {Key} cached as null",
                    typeof(T), key);
                return InsertCacheObject(addToCacheLoader, cacheKey, duration);
            }
        }

        public async Task<T> CacheAsync<T>(Func<Task<T>> addToCacheLoader, string key,
            int duration = CacheDuration.CacheLong,
            bool refreshObject = false)
        {
            var cacheKey = GenerateKey<T>(key);
            try
            {
                if (refreshObject)
                {
                    _logger.LogInformation("object of {Type} with {Key} refreshed for {Duration}",
                        typeof(T), key, duration);
                    return await InsertCacheObjectAsync(addToCacheLoader, cacheKey, duration);
                }

                if (!_memoryCache.TryGetValue(cacheKey, out T cachedObject))
                    return cachedObject == null
                        ? await InsertCacheObjectAsync(addToCacheLoader, cacheKey, duration)
                        : cachedObject;
                _logger.LogInformation("object of {Type} with {Key} was fetched from cache",
                    typeof(T), key);
                return cachedObject;
            }
            catch (NullReferenceException)
            {
                // When T is a struct cachedObject cannot be null because structs are value types
                _logger.LogInformation("object of {Type} with {Key} cached as null",
                    typeof(T), key);
                return await InsertCacheObject(addToCacheLoader, cacheKey, duration);
            }
        }

        private T InsertCacheObject<T>(Func<T> addToCacheLoader, string cacheKey, int duration = CacheDuration.CacheLow)
        {
            var cachedObject = addToCacheLoader.Invoke();

            if (duration == CacheDuration.NoCache) return cachedObject;

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(duration));
            _memoryCache.Set(cacheKey, cachedObject, cacheEntryOptions);
            _logger.LogInformation("object of {Type} with {Key} cached for {Duration}",
                typeof(T), cacheKey, duration);

            return cachedObject;
        }

        private async Task<T> InsertCacheObjectAsync<T>(Func<Task<T>> addToCacheLoader, string cacheKey,
            int duration = CacheDuration.CacheLow)
        {
            var cachedObject = await addToCacheLoader.Invoke();

            if (duration == CacheDuration.NoCache) return cachedObject;

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(duration));
            _memoryCache.Set(cacheKey, cachedObject, cacheEntryOptions);
            _logger.LogInformation("object of {Type} with {Key} cached for {Duration}",
                typeof(T), cacheKey, duration);

            return cachedObject;
        }

        #region Key Management

        private static string GenerateKey<T>(params object[] keys) =>
            $"{Hashing.Create(typeof(T).ToString())}_{string.Join("_", keys)}";

        #endregion
    }
}