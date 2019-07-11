using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Cryptography;
using System.Text;

namespace Simplisity
{
    public class CacheUtils
    {

        #region "cache"

        public static object GetCache(string cacheKey)
        {
            cacheKey = GetMd5Hash(cacheKey);

            ObjectCache cache = MemoryCache.Default;
            if (cache.GetCacheItem(cacheKey) == null)
            {
                return null;
            }
            return cache.GetCacheItem(cacheKey).Value;
        }

        public static void SetCache(string cacheKey, object objObject)
        {
            if (objObject != null)
            {
                RemoveCache(cacheKey);

                cacheKey = GetMd5Hash(cacheKey);

                ObjectCache cache = MemoryCache.Default;
                CacheItemPolicy policy = new CacheItemPolicy();
                var cacheData = new CacheItem(cacheKey, objObject);
                cache.Set(cacheData, policy);

            }
        }

        public static void RemoveCache(string cacheKey)
        {
            cacheKey = GetMd5Hash(cacheKey);

            ObjectCache cache = MemoryCache.Default;
            cache.Remove(cacheKey);
        }

        public static void ClearAllCache()
        {
            try
            {
                ObjectCache cache = MemoryCache.Default;
                List<string> cacheKeys = cache.Select(kvp => kvp.Key).ToList();
                foreach (string cacheKey in cacheKeys)
                {
                    cache.Remove(cacheKey);
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }

        private static string GetMd5Hash(string input)
        {
            var md5 = MD5.Create();
            var inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);
            var sb = new StringBuilder();
            foreach (byte t in hash)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString();
        }


        #endregion

    }
}
