using Simplisity;
using System;
using System.Collections.Generic;
using System.Text;
using DNNrocketAPI.Components;
using System.IO;
using System.Xml;
using System.Linq;

namespace DNNrocketAPI.Components
{
    public class SystemLimpet
    {
        public SystemLimpet(string systemKey)
        {
            Exists = true;
            Record = new SimplisityRecord();
            SystemKey = systemKey.ToLower();
            GetSystemRelPath();
            Record = (SimplisityRecord)CacheUtilsDNN.GetCache(systemKey); // use cache for SystemKey read.
            if (Record == null || Record.GetXmlProperty("genxml/systemkey") == "")
            {
                Record = GetConfig();
                CacheUtilsDNN.SetCache(systemKey, Record);
            }
            InitSystem();
        }
        private SimplisityRecord GetConfig()
        {
            Record = new SimplisityRecord();
            var fileStr = FileUtils.ReadFile(SystemMapPath + "\\system.rules"); 

            // WE do not have a portalid to get the correct portal in the Scheduler.
            //if (fileStr == "") fileStr = FileUtils.ReadFile(PortalUtils.HomeDirectoryMapPath().TrimEnd('\\') + "\\system.rules");
            
            if (fileStr != "")
            {
                Record.XMLData = fileStr;
                var idxList = Record.GetRecordList("sqlindex");
                foreach (var idx in idxList)
                {
                    DNNrocketUtils.UpdateSqlIndex(idx);
                }
            }
            else
                Exists = false;

            if (Exists)
            {
                // Load Providers/Plugins
                var dirPath = SystemMapPath + "\\Plugins";
                if (Directory.Exists(dirPath))
                {
                    foreach (var dirSubPath in Directory.GetDirectories(dirPath))
                    {
                        var files = Directory.GetFiles(dirSubPath, "*.rules");
                        foreach (var f in files)
                        {
                            var fileStr2 = FileUtils.ReadFile(f);
                            var sRec = new SimplisityRecord();
                            sRec.XMLData = fileStr2;

                            var pList = sRec.GetRecordList("providerdata");
                            foreach (var p in pList)
                            {
                                Record.AddRecordListItem("providerdata", p);
                            }

                            var iList = sRec.GetRecordList("interfacedata");
                            foreach (var i in iList)
                            {
                                Record.AddRecordListItem("interfacedata", i);
                            }

                            var idxList = sRec.GetRecordList("sqlindex");
                            foreach (var idx in idxList)
                            {
                                DNNrocketUtils.UpdateSqlIndex(idx);
                            }
                        }
                        //Do NOT delete the plugin files, they are used to build the system data
                    }
                }
            }

            return Record;
        }

        private void InitSystem()
        {
            EventList = new List<RocketInterface>();
            SchedulerList = new List<RocketInterface>();
            HandleBarsList = new List<RocketInterface>();
            RazorList = new List<RocketInterface>();
            ProviderList = new List<RocketInterface>();
            InterfaceList = new Dictionary<string, RocketInterface>();
            Settings = new Dictionary<string, string>();
           
            var l = Record.GetRecordList("interfacedata");
            foreach (var r in l)
            {
                var rocketInterface = new RocketInterface(new SimplisityInfo(r));
                if (!InterfaceList.ContainsKey(rocketInterface.InterfaceKey)) InterfaceList.Add(rocketInterface.InterfaceKey, rocketInterface);
            }
            var l2 = Record.GetRecordList("settingsdata");
            foreach (var s in l2)
            {
                var key = s.GetXmlProperty("genxml/textbox/name");
                if (key != "" && !Settings.ContainsKey(key)) Settings.Add(key, s.GetXmlProperty("genxml/textbox/value"));
            }
            var l3 = Record.GetRecordList("providerdata");
            foreach (var r in l3)
            {
                var rocketInterface = new RocketInterface(new SimplisityInfo(r));
                if (rocketInterface.IsActive && rocketInterface.IsProvider("eventprovider") && rocketInterface.Assembly != "" && rocketInterface.NameSpaceClass != "")
                {
                    EventList.Add(rocketInterface);
                }
                if (rocketInterface.IsActive && rocketInterface.IsProvider("scheduler") && rocketInterface.Assembly != "" && rocketInterface.NameSpaceClass != "")
                {
                    SchedulerList.Add(rocketInterface);
                }
                if (rocketInterface.IsActive && rocketInterface.IsProvider("handlebars") && rocketInterface.Assembly != "" && rocketInterface.NameSpaceClass != "")
                {
                    HandleBarsList.Add(rocketInterface);
                }
                if (rocketInterface.IsActive && rocketInterface.IsProvider("razor") && rocketInterface.Assembly != "" && rocketInterface.NameSpaceClass != "")
                {
                    RazorList.Add(rocketInterface);
                }
                if (rocketInterface.IsActive && !rocketInterface.IsProvider("") && rocketInterface.Assembly != "" && rocketInterface.NameSpaceClass != "")
                {
                    ProviderList.Add(rocketInterface);
                }

            }
        }
        /// <summary>
        /// Each system should have a "system.rules", search for this in DNNrocketModules.  If not there assume DNNrocket folder.
        /// </summary>
        private void GetSystemRelPath()
        {
            var f = DNNrocketUtils.MapPath("/Desktopmodules/dnnrocketmodules/" + SystemKey + "/system.rules");
            if (File.Exists(f))
                SystemRelPath = "/Desktopmodules/dnnrocketmodules/" + SystemKey;
            else
                SystemRelPath = "/Desktopmodules/dnnrocket/" + SystemKey;
        }
        public RocketInterface GetProvider(string interfaceKey)
        {
            var s = Record.GetRecordListItem("providerdata", "genxml/textbox/interfacekey", interfaceKey);
            if (s == null) return null;
            return new RocketInterface(new SimplisityInfo(s));
        }
        public SimplisityInfo SystemInfo { get { return new SimplisityInfo(Record); } }
        public SimplisityRecord Record { get; set; }
        public List<RocketInterface> EventList { get; set;}
        public List<RocketInterface> SchedulerList { get; set; }
        public List<RocketInterface> HandleBarsList { get; set; }
        public List<RocketInterface> RazorList { get; set; }        
        public List<RocketInterface> ProviderList { get; set; }
        public bool Exists { get; set; }
        public Dictionary<string, RocketInterface> InterfaceList { get; set; }
        public Dictionary<string, string> Settings { get; set; }
        public string GetSetting(string key)
        {
            if (Settings.ContainsKey(key)) return Settings[key];
            return "";
        }
        public bool HasInterface(string interfaceKey)
        {
            return InterfaceList.ContainsKey(interfaceKey);
        }
        public List<RocketInterface> GetInterfaceList()
        {
            var rtnList = new List<RocketInterface>();
            var s = Record.GetRecordList("interfacedata");
            if (s == null) return null;
            foreach (var i in s)
            {
                var iface = new RocketInterface(new SimplisityInfo(i));
                rtnList.Add(iface);
            }
            return rtnList;
        }
        public List<RocketInterface> GetGroupInterfaceList(string groupref, bool checksecurity = false)
        {
            var rtnList = new List<RocketInterface>();
            var s = Record.GetRecordList("interfacedata");
            if (s == null) return null;
            foreach (var i in s)
            {
                if (i.GetXmlProperty("genxml/dropdownlist/group") == groupref)
                {
                    var iface = new RocketInterface(new SimplisityInfo(i));
                    if (checksecurity)
                    {
                        if (iface.SecurityCheckUser(PortalUtils.GetCurrentPortalId(), UserUtils.GetCurrentUserId()))
                        {
                            rtnList.Add(iface);
                        }
                    }
                    else
                    {
                        rtnList.Add(iface);
                    }
                }
            }
            return rtnList;
        }

        public RocketInterface GetInterface(string interfaceKey)
        {
            var s = Record.GetRecordListItem("interfacedata", "genxml/textbox/interfacekey", interfaceKey);
            if (s == null) return null;
            return new RocketInterface(new SimplisityInfo(s));
        }
        public Dictionary<string,string> GetGroups(bool emptyEntry = false)
        {
            var rtnList = new Dictionary<string, string>();
            if (emptyEntry) rtnList.Add("", ""); // add empty for root level.

            var s = Record.GetRecordList("groupsdata");
            if (s == null) return null;
            foreach (var i in s)
            {
                rtnList.Add(i.GetXmlProperty("genxml/textbox/groupref"), i.GetXmlProperty("genxml/textbox/groupicon"));
            }
            return rtnList;
        }
        public string SystemKey { get; set; }
        public string SystemName
        {
            get { return Record.GetXmlProperty("genxml/systemname"); }
            set { Record.SetXmlProperty("genxml/systemname", value); }
        }
        public bool IsPlugin { get { return Record.GetXmlPropertyBool("genxml/plugin"); } }
        public string Icon
        {
            get { return Record.GetXmlProperty("genxml/icon"); }
            set { Record.SetXmlProperty("genxml/icon", value); }
        }
        public bool Active
        {
            get { return Record.GetXmlPropertyBool("genxml/active"); }
        }        
        public bool LogTracking
        {
            get { return Record.GetXmlPropertyBool("genxml/logtracking"); }
        }
        
        public string DatabaseTable
        {
            get
            {
                var dbt = Record.GetXmlProperty("genxml/databasetable");
                if (dbt == "") dbt = "DNNrocket";
                return dbt;
            }
        }
        public string AdminUrl
        {
            get { return SystemRelPath + "/" + SystemKey + "/admin.html"; }
        }
        public string SystemRelPath { get; set; }
        public string SystemMapPath
        {
            get { return DNNrocketUtils.MapPath(SystemRelPath); }
        }
        public string ReleaseNumber { get { return Record.GetXmlProperty("genxml/releasenumber"); } }

    }
}
