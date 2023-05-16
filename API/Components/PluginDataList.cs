using DNNrocketAPI;
using DNNrocketAPI.Components;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Rocket.AppThemes.Components
{

    public class PluginDataList
    {
        private string _listCacheKey;
        private string _pluginCacheGroupId;
        public PluginDataList(bool useCache = true)
        {
            try
            {
                SystemList = new SystemLimpetList();

                _pluginCacheGroupId = "plugins";
                _listCacheKey = "PluginList" + UserUtils.GetCurrentUserId();

                if (useCache)
                {
                    PluginList = (List<SimplisityRecord>)CacheUtils.GetCache(_listCacheKey);
                }
                if (PluginList == null) PopulateList();
            }
            catch (Exception exc)
            {
                ErrorMsg = exc.ToString();
                Error = true;
            }
        }
        public void PopulateList()
        {
            //PluginList = new List<SimplisityRecord>();
            //var ftpConnect = new FtpConnect("plugins");
            //if (ftpConnect.IsValid)
            //{
            //    var l = ftpConnect.DownloadAppThemeXmlIndexList();
            //    List<SimplisityRecord> SortedList = l.OrderBy(o => o.GetXmlProperty("genxml/hidden/name")).ToList();
            //    foreach (SimplisityRecord a in SortedList)
            //    {
            //        PluginList.Add(a);
            //    }
            //}
            //else
            //{
            //    Error = true;
            //    ErrorMsg = "";
            //}
            //CacheUtils.SetCache(_listCacheKey, PluginList);
        }
        public void ClearCache()
        {
            CacheUtils.RemoveCache(_listCacheKey);
        }
        public List<SimplisityRecord> PluginList { get; set; }
        public SystemLimpetList SystemList { get; set; }
        public bool Error { get; set; }
        public string ErrorMsg { get; set; }

    }

    public class PluginData
    {

        public PluginData(string systemKey)
        {
            SystemKey = systemKey;
            DownloadPlugin();
            System = SystemSingleton.Instance(SystemKey);

        }
        private void DownloadPlugin()
        {


        }


        public SystemLimpet System { get; set; }
        public string SystemKey { get; set; }

    }

}
