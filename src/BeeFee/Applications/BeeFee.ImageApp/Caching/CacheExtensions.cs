using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BeeFee.ImageApp.Caching
{
    public static class CacheExtensions
    {
        private static int DefaultCacheTimeMinutes => 60;

        public static T Get<T>(this MemoryCacheManager cacheManager, string key, Func<T> acquire)
        => Get(cacheManager, key, DefaultCacheTimeMinutes, acquire);

        public static T Get<T>(this MemoryCacheManager cacheManager, string key, int cacheTime, Func<T> acquire)
        {
            if (cacheManager.IsSet(key))
                return cacheManager.Get<T>(key);

            var result = acquire();

            if (cacheTime > 0)
                cacheManager.Set(key, result, cacheTime);

            return result;
        }

        public static void RemoveByPattern(this MemoryCacheManager cacheManager, string pattern, IEnumerable<string> keys)
        {
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var matchesKeys = keys.Where(key => regex.IsMatch(key)).ToList();

            matchesKeys.ForEach(cacheManager.Remove);
        }
    }
}
