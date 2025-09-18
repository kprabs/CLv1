using Microsoft.Extensions.Caching.Memory;

namespace CoreLib.API.Helpers
{
    public class UserCacheHelper(IMemoryCache userMemCache) : ICacheHelper  {
        
        public bool TryGetCachedObject(string key, out object cacheValue)
        {
            return userMemCache.TryGetValue(key, out cacheValue);
        }

        public bool TryGetCachedObject(int key, out dynamic cacheValue)
        {
            return userMemCache.TryGetValue(key, out cacheValue);
        }

        public object SetCachedObject(string key, object Value)
        {
            userMemCache.Set(key, Value);
            return Value;
        }

        public bool RemoveCachedObject(string key)
        {
            userMemCache.Remove(key);
            return true;
        }
    }

    public interface ICacheHelper
    {
        public object SetCachedObject(string key, object Value);
        public bool TryGetCachedObject(string key, out object cacheValue);
        public bool TryGetCachedObject(int key, out dynamic cacheValue);
        public bool RemoveCachedObject(string key);
    }
}
