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

        public static string GetCache(string cacheKey)
        {
            var cacheData = (string)CacheUtilsDNN.GetCache(cacheKey);
            if (cacheData == null)
            {
                var cacheFile = GetMd5Hash(cacheKey);
                var cacheDataStr = FileUtils.ReadFile(PortalUtils.TempDirectoryMapPath().Trim('\\') + "\\cache\\" + cacheFile);
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

                if (!Directory.Exists(PortalUtils.TempDirectoryMapPath().Trim('\\') + "\\cache"))
                {
                    Directory.CreateDirectory(PortalUtils.TempDirectoryMapPath().Trim('\\') + "\\cache");
                }
                var cacheFile = GetMd5Hash(cacheKey);
                FileUtils.SaveFile(PortalUtils.TempDirectoryMapPath().Trim('\\') + "\\cache\\" + cacheFile, objObject);
            }
        }

        public static void RemoveCache(string cacheKey)
        {
            CacheUtilsDNN.RemoveCache(cacheKey);
            var cacheFile = GetMd5Hash(cacheKey);

            if (File.Exists(PortalUtils.TempDirectoryMapPath().Trim('\\') + "\\cache\\" + cacheFile))
            {
                File.Delete(PortalUtils.TempDirectoryMapPath().Trim('\\') + "\\cache\\" + cacheFile);
            }
        }
        public static void ClearFileCacheAllPortals()
        {
            try
            {
                var pList = PortalUtils.GetPortals();
                foreach (var portalid in pList)
                {
                    ClearFileCache(portalid);
                }
                CacheUtilsDNN.ClearAllCache();
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }

        public static void ClearAllCache(int portalid = -1)
        {
            try
            {
                ClearFileCache(portalid);
                CacheUtilsDNN.ClearAllCache();
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }

        public static void ClearFileCache(int portalid = -1)
        {
            try
            {
                if (Directory.Exists(PortalUtils.TempDirectoryMapPath(portalid).Trim('\\') + "\\cache"))
                {
                    Directory.Delete(PortalUtils.TempDirectoryMapPath(portalid).Trim('\\') + "\\cache", true);
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
