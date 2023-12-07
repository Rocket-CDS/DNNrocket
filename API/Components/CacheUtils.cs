using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Cryptography;
using System.Text;

namespace DNNrocketAPI.Components
{
    public class CacheUtils
    {

        #region "cache"

        public static object GetCache(string cacheKey, string groupid = "")
        {
            if (String.IsNullOrEmpty(cacheKey)) return null; // no cache if no cacheKey
            var groupKey = "_" + groupid;
            var cacheGroupKey = GetMd5Hash(cacheKey) + groupKey;
            return CacheUtilsDNN.GetCache(cacheGroupKey);
        }
        public static List<object> GetGroupCache(string groupid)
        {
            try
            {
                var groupKey = "_" + groupid;
                var groupCacheKeys = (List<string>)CacheUtilsDNN.GetCache(groupKey);
                var rtnList = new List<object>();
                foreach (var gck in groupCacheKeys)
                {
                    rtnList.Add(CacheUtilsDNN.GetCache(gck));
                }
                return rtnList;
            }
            catch (Exception ex)
            {
                LogUtils.LogException(ex);
                LogUtils.LogSystem("ERROR GetGroupCache(string groupid) : " + ex.Message);
                return null;
            }
        }
        public static void SetCache(string cacheKey, object objObject, string groupid = "")
        {
            if (objObject != null && !String.IsNullOrEmpty(cacheKey))
            {
                var groupKey = "_" + groupid;
                var cacheGroupKey = GetMd5Hash(cacheKey) + groupKey;
                var groupCacheKeys = (List<string>)CacheUtilsDNN.GetCache(groupKey);
                if (groupCacheKeys == null) groupCacheKeys = new List<string>();

                CacheUtilsDNN.SetCache(cacheGroupKey, objObject);
                if (!groupCacheKeys.Contains(cacheGroupKey))
                {
                    groupCacheKeys.Add(cacheGroupKey);
                    CacheUtilsDNN.SetCache(groupKey, groupCacheKeys);
                }
            }
        }
        public static void RemoveCache(string cacheKey, string groupid = "")
        {
            var groupKey = "_" + groupid;
            var cacheGroupKey = GetMd5Hash(cacheKey) + groupKey;
            CacheUtilsDNN.RemoveCache(cacheGroupKey);

            var groupCacheKeys = (List<string>)CacheUtilsDNN.GetCache(groupKey);
            if (groupCacheKeys != null)
            {
                if (groupCacheKeys.Contains(cacheGroupKey))
                {
                    groupCacheKeys.Remove(cacheGroupKey);
                }
                CacheUtilsDNN.SetCache(groupKey, groupCacheKeys);
            }
        }

        public static void ClearAllCache(string groupid = "")
        {
            try
            {
                if (groupid == "")
                {
                    CacheUtilsDNN.ClearAllCache();
                }
                else
                {
                    var groupKey = "_" + groupid;
                    var groupCacheKeys = (List<string>)CacheUtilsDNN.GetCache(groupKey);
                    if (groupCacheKeys != null)
                    {
                        foreach (var gck in groupCacheKeys)
                        {
                            CacheUtilsDNN.RemoveCache(gck);
                        }
                        CacheUtilsDNN.RemoveCache(groupKey);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtils.LogSystem("ERROR ClearAllCache(string groupid) : " + ex.Message);
            }
        }

        private static string GetMd5Hash(string input)
        {
            return Md5HashCalc(input);
        }

        public static string Md5HashCalc(string input)
        {
            var inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = MD5.Create().ComputeHash(inputBytes);
            return BitConverter.ToString(hash).Replace("-", "");
        }

        #endregion

    }
}
