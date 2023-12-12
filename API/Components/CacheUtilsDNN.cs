using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Cryptography;
using System.Text;

namespace DNNrocketAPI.Components
{
    [Obsolete("CacheUtilsDNN is deprecated, please use CacheUtils instead.")]
    public class CacheUtilsDNN
    {

        #region "cache "


        [Obsolete("CacheUtilsDNN is deprecated, please use CacheUtils instead.")]
        public static object GetCache(string cacheKey)
        {
            cacheKey = GetMd5Hash(cacheKey);
            return DNNrocketUtils.GetCache(cacheKey);
        }
        [Obsolete("CacheUtilsDNN is deprecated, please use CacheUtils instead.")]
        public static void SetCache(string cacheKey, object objObject)
        {
            if (objObject != null)
            {
                RemoveCache(cacheKey);
                cacheKey = GetMd5Hash(cacheKey);
                DNNrocketUtils.SetCache(cacheKey, objObject);
            }
        }
        [Obsolete("CacheUtilsDNN is deprecated, please use CacheUtils instead.")]
        public static void RemoveCache(string cacheKey)
        {
            cacheKey = GetMd5Hash(cacheKey);
            DNNrocketUtils.RemoveCache(cacheKey);
        }
        [Obsolete("CacheUtilsDNN is deprecated, please use CacheUtils instead.")]
        public static void ClearAllCache()
        {
            try
            {
                DNNrocketUtils.ClearAllCache();
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }
        private static string GetMd5Hash(string input)
        {
            if (input.Length < 200) return input;  // Only use MD5 if we have a large key. It throws stackoverflow from scheudler. + I'm unsure about the collisions that can happen.

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
