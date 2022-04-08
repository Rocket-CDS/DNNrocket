using DNNrocketAPI.Components;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RocketPortal.Components
{
    public class WebsiteBuild
    {
        private List<SimplisityRecord> _sList;
        public WebsiteBuild()
        {
            ConfigFileList = Directory.GetFiles(DNNrocketUtils.MapPath("/DesktopModules/DNNrocket/RocketPortal/WebsiteBuilds"));
            _sList = (List<SimplisityRecord>)CacheUtils.GetCache("RocketWebsiteBuilds");
            if (_sList == null)
            {
                _sList = new List<SimplisityRecord>();
                foreach (var f in ConfigFileList)
                {
                    var strXml = FileUtils.ReadFile(f);
                    var sRec = new SimplisityRecord();
                    try
                    {
                        sRec.XMLData = strXml;
                        sRec.SetXmlProperty("genxml/filename", Path.GetFileName(f));
                        _sList.Add(sRec);
                    }
                    catch (Exception ex)
                    {
                        LogUtils.LogException(ex);
                    }
                }
                CacheUtils.SetCache("RocketWebsiteBuilds", _sList);
            }
        }

        public string[] ConfigFileList { get; set; }
        public List<SimplisityRecord> ConfigList { get { return _sList; }  }

    }
}
