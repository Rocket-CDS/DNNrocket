using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Cryptography;
using System.Text;

namespace DNNrocketAPI.Components
{
    public class CacheUtilsDNN
    {

        #region "cache "


        public static object GetCache(string cacheKey)
        {
            cacheKey = GetMd5Hash(cacheKey);
            return DNNrocketUtils.GetCache(cacheKey);
        }
        public static void SetCache(string cacheKey, object objObject)
        {
            if (objObject != null)
            {
                RemoveCache(cacheKey);
                cacheKey = GetMd5Hash(cacheKey);
                DNNrocketUtils.SetCache(cacheKey, objObject);
            }
        }
        public static void RemoveCache(string cacheKey)
        {
            cacheKey = GetMd5Hash(cacheKey);
            DNNrocketUtils.RemoveCache(cacheKey);
        }

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
