using Newtonsoft.Json.Linq;
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
            return DNNrocketUtils.GetCache(cacheGroupKey);
        }
        public static List<object> GetGroupCache(string groupid)
        {
            try
            {
                var groupKey = "_" + groupid;
                var groupCacheKeys = (List<string>)DNNrocketUtils.GetCache(groupKey);
                var rtnList = new List<object>();
                if (groupCacheKeys != null)
                {
                    foreach (var gck in groupCacheKeys)
                    {
                        rtnList.Add(DNNrocketUtils.GetCache(gck));
                    }
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
                var groupCacheKeys = (List<string>)DNNrocketUtils.GetCache(groupKey);
                if (groupCacheKeys == null) groupCacheKeys = new List<string>();

                DNNrocketUtils.SetCache(cacheGroupKey, objObject);
                if (!groupCacheKeys.Contains(cacheGroupKey))
                {
                    groupCacheKeys.Add(cacheGroupKey);
                    DNNrocketUtils.SetCache(groupKey, groupCacheKeys);
                }
            }
        }
        public static void RemoveCache(string cacheKey, string groupid = "")
        {
            var groupKey = "_" + groupid;
            var cacheGroupKey = GetMd5Hash(cacheKey) + groupKey;
            DNNrocketUtils.RemoveCache(cacheGroupKey);

            var groupCacheKeys = (List<string>)DNNrocketUtils.GetCache(groupKey);
            if (groupCacheKeys != null)
            {
                if (groupCacheKeys.Contains(cacheGroupKey))
                {
                    groupCacheKeys.Remove(cacheGroupKey);
                }
                DNNrocketUtils.SetCache(groupKey, groupCacheKeys);
            }
        }

        public static void ClearAllCache(string groupid = "")
        {
            try
            {
                if (groupid == "")
                {
                    // we want to keep "testing" group cache
                    var testingObjs = new Dictionary<string, object>();
                    var testing = (List<string>)DNNrocketUtils.GetCache("_testing"); 
                    if (testing != null)
                    {
                        foreach (var k in testing)
                        {
                            var obj = DNNrocketUtils.GetCache(k);
                            testingObjs.Add(k, obj);
                        }
                    }

                    DNNrocketUtils.ClearAllCache();

                    // rebuild "testing" group cache
                    var groupCacheKeys = new List<string>();
                    foreach (var o in testingObjs)
                    {
                        DNNrocketUtils.SetCache(o.Key, o.Value);
                        if (!groupCacheKeys.Contains(o.Key))
                        {
                            groupCacheKeys.Add(o.Key);
                            DNNrocketUtils.SetCache("_testing", groupCacheKeys);
                        }
                    }
                }
                else
                {
                    var groupKey = "_" + groupid;
                    var groupCacheKeys = (List<string>)DNNrocketUtils.GetCache(groupKey);
                    if (groupCacheKeys != null)
                    {
                        foreach (var gck in groupCacheKeys)
                        {
                            DNNrocketUtils.RemoveCache(gck);
                        }
                        DNNrocketUtils.RemoveCache(groupKey);
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

        #region "testing cache"
        public static DateTime DateTimeNow()
        {
            var rtn = CacheUtils.GetCache("systemdate", "testing");
            if (rtn == null) return DateTime.Now;
            return (DateTime)rtn;
        }
        public static void DateTimeNowSet(DateTime value)
        {
            SetCache("systemdate", value, "testing");
        }
        public static void DateTimeNowRemove()
        {
            RemoveCache("systemdate", "testing");
        }
        public static void SetTestingCache(string key, object obj)
        {
            SetCache(key, obj, "testing");
        }
        public static object GetTestingCache(string key)
        {
            return GetCache(key, "testing");
        }
        public static void ClearTestingCache()
        {
            ClearAllCache("testing");
        }

        public static Dictionary<string, List<string>> TestCache()
        {
            var ListOK = new List<string>();
            var ListFAIL = new List<string>();
            var testmsg = "";

            // START: 1 
            testmsg = "1 - Set and Get Cache (No Group)"; 
            CacheUtils.SetCache("test1", "TEST1");
            var test1 = (string)CacheUtils.GetCache("test1");
            if (test1 == "TEST1")
                ListOK.Add(testmsg);
            else
                ListFAIL.Add(testmsg);

            testmsg = "1 - Remove Cache (No Group)"; 
            CacheUtils.RemoveCache("test1");
            test1 = (string)CacheUtils.GetCache("test1");
            if (test1 == null)
                ListOK.Add(testmsg);
            else
                ListFAIL.Add(testmsg);

            CacheUtils.SetCache("test1", "TEST1"); // set cache for later test


            // --------------------------------------------------------------------------------------------------------
            // START: 2
            testmsg = "2 - Set and Get Cache (Group: 2)"; 
            CacheUtils.SetCache("test2", "TEST2", "2");
            var test2 = (string)CacheUtils.GetCache("test2","2");
            if (test2 == "TEST2")
                ListOK.Add(testmsg);
            else
                ListFAIL.Add(testmsg);

            testmsg = "2 - Get Cache (No Group)";
            test2 = (string)CacheUtils.GetCache("test2");
            if (test2 == null)
                ListOK.Add(testmsg);
            else
                ListFAIL.Add(testmsg);

            testmsg = "2 - Remove Cache (Group: 2)";
            CacheUtils.RemoveCache("test2", "2");
            test2 = (string)CacheUtils.GetCache("test2", "2");
            if (test2 == null)
                ListOK.Add(testmsg);
            else
                ListFAIL.Add(testmsg);

            testmsg = "2 - test1 Cache still exists after test2 remove";
            test1 = (string)CacheUtils.GetCache("test1");
            if (test1 == "TEST1")
                ListOK.Add(testmsg);
            else
                ListFAIL.Add(testmsg);


            testmsg = "2 - Clear Cache (Group: 2)";
            CacheUtils.ClearAllCache("2");
            test2 = (string)CacheUtils.GetCache("test2", "2");
            if (test2 == null)
                ListOK.Add(testmsg);
            else
                ListFAIL.Add(testmsg);

            testmsg = "2 - test1 Cache still exists after clearcache group 2";
            test1 = (string)CacheUtils.GetCache("test1");
            if (test1 == "TEST1")
                ListOK.Add(testmsg);
            else
                ListFAIL.Add(testmsg);

            CacheUtils.ClearAllCache();
            testmsg = "2 - test1 Cache removed after clearcache all";
            test1 = (string)CacheUtils.GetCache("test1");
            if (test1 == null)
                ListOK.Add(testmsg);
            else
                ListFAIL.Add(testmsg);
            // --------------------------------------------------------------------------------------------------------
            // START: 3
            CacheUtils.SetCache("test3", "TEST31", "1");
            CacheUtils.SetCache("test3", "TEST32", "2");
            CacheUtils.SetCache("test3", "TEST33", "3");
            testmsg = "3 - No cache in any non-group";
            if (CacheUtils.GetCache("test3") == null)
                ListOK.Add(testmsg);
            else
                ListFAIL.Add(testmsg);
            testmsg = "3 - cache in group1";
            if (CacheUtils.GetCache("test3", "1").ToString() == "TEST31")
                ListOK.Add(testmsg);
            else
                ListFAIL.Add(testmsg);
            testmsg = "3 - cache in group2";
            if (CacheUtils.GetCache("test3", "2").ToString() == "TEST32")
                ListOK.Add(testmsg);
            else
                ListFAIL.Add(testmsg);
            testmsg = "3 - cache in group3";
            if (CacheUtils.GetCache("test3", "3").ToString() == "TEST33")
                ListOK.Add(testmsg);
            else
                ListFAIL.Add(testmsg);
            testmsg = "3 - no cache in group4";
            if (CacheUtils.GetCache("test3", "4") == null)
                ListOK.Add(testmsg);
            else
                ListFAIL.Add(testmsg);

            CacheUtils.SetCache("test3", "TEST31r", "1");
            CacheUtils.SetCache("test3", "TEST32r", "2");
            CacheUtils.SetCache("test3", "TEST33r", "3");
            testmsg = "3 - rewrite cache in group1";
            if (CacheUtils.GetCache("test3", "1").ToString() == "TEST31r")
                ListOK.Add(testmsg);
            else
                ListFAIL.Add(testmsg);
            testmsg = "3 - rewrite cache in group2";
            if (CacheUtils.GetCache("test3", "2").ToString() == "TEST32r")
                ListOK.Add(testmsg);
            else
                ListFAIL.Add(testmsg);
            testmsg = "3 - rewrite cache in group3";
            if (CacheUtils.GetCache("test3", "3").ToString() == "TEST33r")
                ListOK.Add(testmsg);
            else
                ListFAIL.Add(testmsg);

            CacheUtils.ClearAllCache();
            testmsg = "3 - no cache in group1 after clearallcache";
            if (CacheUtils.GetCache("test3", "1") == null)
                ListOK.Add(testmsg);
            else
                ListFAIL.Add(testmsg);
            testmsg = "3 - no cache in group2 after clearallcache";
            if (CacheUtils.GetCache("test3", "2") == null)
                ListOK.Add(testmsg);
            else
                ListFAIL.Add(testmsg);
            testmsg = "3 - no cache in group3 after clearallcache";
            if (CacheUtils.GetCache("test3", "3") == null)
                ListOK.Add(testmsg);
            else
                ListFAIL.Add(testmsg);

            // --------------------------------------------------------------------------------------------------------

            var rtn = new Dictionary<string, List<string>>();
            rtn.Add("FAIL", ListFAIL);
            rtn.Add("OK", ListOK);
            return rtn;
        }

        #endregion
    }
}
