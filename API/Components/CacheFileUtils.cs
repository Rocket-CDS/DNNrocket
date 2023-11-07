using DNNrocketAPI;
using DNNrocketAPI.Components;
using Newtonsoft.Json.Linq;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Cryptography;
using System.Text;

namespace DNNrocketAPI.Components
{
    public class CacheFileUtils
    {

        #region "cache"

        public static string GetCache(string cacheKey)
        {
            var cacheData = (string)CacheUtils.GetCache(cacheKey);
            if (cacheData == null)
            {
                var cacheFile = GetMd5Hash(cacheKey);
                var cacheDataStr = FileUtils.ReadFile(PortalUtils.TempDirectoryMapPath().Trim('\\') + "\\cache\\" + cacheFile);
                if (String.IsNullOrEmpty(cacheDataStr)) return "";
                CacheUtils.SetCache(cacheKey, cacheDataStr);
                return cacheDataStr;
            }
            return cacheData;
        }

        public static void SetCache(string cacheKey, string objObject)
        {
            if (!String.IsNullOrEmpty(objObject))
            {
                CacheUtils.SetCache(cacheKey, objObject);

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
            CacheUtils.RemoveCache(cacheKey);
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
                CacheUtils.ClearAllCache();
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
                CacheUtils.ClearAllCache();
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

        #region "Binary"

        /// <summary>
        /// Writes the given object instance to a binary file.
        /// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
        /// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the XML file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the XML file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static void WriteToBinaryFile<T>(string cacheKey, T objectToWrite, bool append = false)
        {
            var filePath = PortalUtils.TempDirectoryMapPath().Trim('\\') + "\\cache\\" + cacheKey;

            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }

        /// <summary>
        /// Reads an object instance from a binary file.
        /// </summary>
        /// <typeparam name="T">The type of object to read from the XML.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the binary file.</returns>
        public static T ReadFromBinaryFile<T>(string cacheKey)
        {
            var filePath = PortalUtils.TempDirectoryMapPath().Trim('\\') + "\\cache\\" + cacheKey;
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }

        #endregion
    }
}
