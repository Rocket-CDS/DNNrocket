using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Rocket.AppThemes.Componants
{

    public class PluginDataList
    {
        private string _listCacheKey;
        private string _pluginCacheGroupId;
        public PluginDataList(bool useCache = true)
        {
            try
            {
                SystemList = new SystemDataList();

                _pluginCacheGroupId = "plugins";
                _listCacheKey = "PluginList" + DNNrocketUtils.GetCurrentUserId();
                
                if (useCache)
                {
                    PluginList = (List<SimplisityRecord>)CacheUtils.GetCache(_listCacheKey, _pluginCacheGroupId);
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
            PluginList = new List<SimplisityRecord>();
            var ftpConnect = new FtpConnect("plugins");
            if (ftpConnect.IsValid)
            {
                var l = ftpConnect.DownloadAppThemeXmlIndexList();
                List<SimplisityRecord> SortedList = l.OrderBy(o => o.GetXmlProperty("genxml/hidden/name")).ToList();
                foreach (SimplisityRecord a in SortedList)
                {
                    PluginList.Add(a);
                }
            }
            else
            {
                Error = true;
                ErrorMsg = "";
            }
            CacheUtils.SetCache(_listCacheKey, PluginList, _pluginCacheGroupId);
        }
        public void ClearCache()
        {
            CacheUtils.RemoveCache(_listCacheKey, _pluginCacheGroupId);
        }
        public List<SimplisityRecord> PluginList { get; set; }
        public SystemDataList SystemList { get; set; }
        public bool Error { get; set; }
        public string ErrorMsg { get; set; }

    }

}
