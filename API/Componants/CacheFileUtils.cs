using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Cryptography;
using System.Text;

namespace DNNrocketAPI.Componants
{
    public class CacheFileUtils
    {

        #region "cache"

        public static string GetCache(string cacheKey, string groupid = "")
        {
            var cacheData = (string)CacheUtilsDNN.GetCache(cacheKey);
            if (cacheData == null)
            {
                var cacheFile = GetMd5Hash(cacheKey + "_groupid:" + groupid);
                var cacheDataStr = FileUtils.ReadFile(DNNrocketUtils.TempDirectoryMapPath().Trim('\\') + "\\cache\\" + cacheFile);
                if (String.IsNullOrEmpty(cacheDataStr)) return "";
                return cacheDataStr;
            }
            return cacheData;
        }

        public static void SetCache(string cacheKey, string objObject)
        {
            if (!String.IsNullOrEmpty(objObject))
            {
                CacheUtilsDNN.SetCache(cacheKey, objObject);

                if (!Directory.Exists(DNNrocketUtils.TempDirectoryMapPath().Trim('\\') + "\\cache"))
                {
                    Directory.CreateDirectory(DNNrocketUtils.TempDirectoryMapPath().Trim('\\') + "\\cache");
                }
                var cacheFile = GetMd5Hash(cacheKey);
                FileUtils.SaveFile(DNNrocketUtils.TempDirectoryMapPath().Trim('\\') + "\\cache\\" + cacheFile, objObject);
            }
        }

        public static void RemoveCache(string cacheKey)
        {
            CacheUtilsDNN.RemoveCache(cacheKey);
            var cacheFile = GetMd5Hash(cacheKey);

            if (File.Exists(DNNrocketUtils.TempDirectoryMapPath().Trim('\\') + "\\cache\\" + cacheFile))
            {
                File.Delete(DNNrocketUtils.TempDirectoryMapPath().Trim('\\') + "\\cache\\" + cacheFile);
            }
        }

        public static void ClearAllCache()
        {
            try
            {
                ClearFileCache();
                CacheUtilsDNN.ClearAllCache();
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }

        public static void ClearFileCache(string groupid = "")
        {
            try
            {
                if (Directory.Exists(DNNrocketUtils.TempDirectoryMapPath().Trim('\\') + "\\cache"))
                {
                    Directory.Delete(DNNrocketUtils.TempDirectoryMapPath().Trim('\\') + "\\cache", true);
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
