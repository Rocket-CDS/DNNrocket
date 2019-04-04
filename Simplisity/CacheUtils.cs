using System.Collections.Generic;
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

        public static List<string> GetCacheKeys(string groupkey)
        {
            ObjectCache cache = MemoryCache.Default;
            var rtnList = new List<string>();
            var v = GetCache("dnnrocketcache" + groupkey);
            if (v != null)
            {
                rtnList = (List<string>)v;
            }
            return rtnList;
        }
        public static List<string> GetCacheGroups()
        {
            ObjectCache cache = MemoryCache.Default;
            var rtnList = new List<string>();
            var v = GetCache("group_dnnrocketcache");
            if (v != null)
            {
                rtnList = (List<string>)v;
            }
            return rtnList;
        }

        public static void SetCache(string cacheKey, object objObject, string groupkey = "default")
        {
            cacheKey = GetMd5Hash(cacheKey);

            ObjectCache cache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy();
            var cacheData = new CacheItem(cacheKey, objObject);
            cache.Set(cacheData, policy);

            if (groupkey == "") groupkey = "default";

            var l1 = GetCacheKeys(groupkey);
            if (!l1.Contains(cacheKey))
            {
                l1.Add(cacheKey);
                cacheKey = GetMd5Hash("dnnrocketcache" + groupkey) ;
                cacheData = new CacheItem(cacheKey, l1);
                cache.Set(cacheData, policy);
            }

            var gl = GetCacheGroups();
            if (!gl.Contains(groupkey))
            {
                gl.Add(groupkey);
                cacheKey = GetMd5Hash("group_dnnrocketcache");
                cacheData = new CacheItem(cacheKey, gl);
                cache.Set(cacheData, policy);
            }

        }

        public static void RemoveCache(string cacheKey)
        {
            cacheKey = GetMd5Hash(cacheKey);

            ObjectCache cache = MemoryCache.Default;
            cache.Remove(cacheKey);
        }

        public static void ClearCache(string groupkey = "default")
        {
            ObjectCache cache = MemoryCache.Default;
            var l = GetCacheKeys(groupkey);
            foreach (var c in l)
            {
                cache.Remove(c);
            }
            RemoveCache("dnnrocketcache" + groupkey);
        }

        public static void ClearAllCache()
        {
            ObjectCache cache = MemoryCache.Default;
            var gl = GetCacheGroups();
            foreach (var g in gl)
            {
                var l = GetCacheKeys(g);
                foreach (var c in l)
                {
                    if (c != null) cache.Remove(c);
                }
                RemoveCache("dnnrocketcache" + g);
            }
            RemoveCache("group_dnnrocketcache");
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
