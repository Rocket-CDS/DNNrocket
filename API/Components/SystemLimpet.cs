using Simplisity;
using System;
using System.Collections.Generic;
using System.Text;
using DNNrocketAPI.Components;
using System.IO;
using System.Xml;

namespace DNNrocketAPI.Components
{
    public class SystemLimpet
    {
        public SystemLimpet(string systemKey)
        {
            Exists = true;
            Record = new SimplisityRecord();
            SystemKey = systemKey;
            Record = (SimplisityRecord)CacheUtilsDNN.GetCache(systemKey); // use cache for SystemKey read.
            if (Record == null || SystemKey == "")
            {
                Record = GetConfig(systemKey);
                CacheUtilsDNN.SetCache(systemKey, Record);
            }
            InitSystem();
        }
        private SimplisityRecord GetConfig(string systemKey)
        {
            Record = new SimplisityRecord();
            var fileStr = FileUtils.ReadFile(SystemMapPath + "\\" + systemKey + "\\system.rules"); 
            if (fileStr == "") fileStr = FileUtils.ReadFile(PortalUtils.HomeDirectoryMapPath().TrimEnd('\\') + "\\system.rules");
            if (fileStr != "")
                Record.XMLData = fileStr;
            else
                Exists = false;

            return Record;
        }
        private void InitSystem()
        {
            EventList = new List<RocketInterface>();
            SchedulerList = new List<RocketInterface>();            
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
            }
        }

        public SimplisityInfo SystemInfo { get { return new SimplisityInfo(Record); } }
        public SimplisityRecord Record { get; set; }
        public List<RocketInterface> EventList { get; set;}
        public List<RocketInterface> SchedulerList { get; set; }
        public bool Exists { get; set; }
        // Used for sceduler and looping through portals.
        public int PortalId { get; set; }
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
        public List<RocketInterface> GetGroupInterfaceList(string groupref)
        {
            var rtnList = new List<RocketInterface>();
            var s = Record.GetRecordList("interfacedata");
            if (s == null) return null;
            foreach (var i in s)
            {
                if (i.GetXmlProperty("genxml/dropdownlist/group") == groupref)
                {
                    var iface = new RocketInterface(new SimplisityInfo(i));
                    rtnList.Add(iface);
                }
            }
            return rtnList;
        }

        public RocketInterface GetInterface(string interfaceKey)
        {
            var s = Record.GetRecordListItem("interfacedata", "genxml/interfacekey", interfaceKey);
            if (s == null) return null;
            return new RocketInterface(new SimplisityInfo(s));
        }
        public Dictionary<string,string> GetGroups()
        {
            var rtnList = new Dictionary<string, string>();
            var s = Record.GetRecordList("groupsdata");
            if (s == null) return null;
            foreach (var i in s)
            {
                rtnList.Add(i.GetXmlProperty("genxml/textbox/groupref"), i.GetXmlProperty("genxml/textbox/groupicon"));
            }
            return rtnList;
        }
        public string SystemKey
        {
            get { return Record.GetXmlProperty("genxml/systemkey"); }
            set { Record.SetXmlProperty("genxml/systemkey", value); }
        }
        public string SystemName
        {
            get { return Record.GetXmlProperty("genxml/systemname"); }
            set { Record.SetXmlProperty("genxml/systemname", value); }
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
        public string SystemRelPath
        {
            get { return "/Desktopmodules/dnnrocketmodules/" + SystemKey; }
        }
        public string SystemMapPath
        {
            get { return DNNrocketUtils.MapPath(SystemRelPath); }
        }
    }
}
