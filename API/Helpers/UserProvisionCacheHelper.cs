using Microsoft.Extensions.Caching.Memory;

namespace CoreLib.API.Helpers
{
    public class UserProvisionCacheHelper(IMemoryCache userProvisionMemCache) : ICacheHelper
    {
        public bool TryGetCachedObject(string key, out object cacheValue)
        {
            return userProvisionMemCache.TryGetValue(key, out cacheValue);
        }

        public bool TryGetCachedObject(int key, out dynamic cacheValue)
        {
            return userProvisionMemCache.TryGetValue(key, out cacheValue);
        }

        public object SetCachedObject(string key, object Value)
        {
            userProvisionMemCache.Set(key, Value);
            return Value;
        }

        public bool RemoveCachedObject(string key)
        {
            userProvisionMemCache.Remove(key);
            return true;
        }
    }
}
