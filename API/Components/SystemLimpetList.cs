using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using DNNrocketAPI.Components;
using Simplisity;

namespace DNNrocketAPI.Components
{

    public class SystemLimpetList
    {
        private List<SystemLimpet> _systemList;

        public SystemLimpetList()
        {
            _systemList = new List<SystemLimpet>();
            var searchFolder = DNNrocketUtils.MapPath("/DesktopModules/DNNrocketModules");
            if (Directory.Exists(searchFolder)) scanFolderForRules(searchFolder);
            searchFolder = DNNrocketUtils.MapPath("/DesktopModules/DNNrocket");
            if (Directory.Exists(searchFolder)) scanFolderForRules(searchFolder);
        }

        private void scanFolderForRules(string searchFolder)
        {
            var dl = Directory.GetDirectories(searchFolder);
            foreach (var d in dl)
            {
                var systemFile = d + "\\system.rules";
                if (File.Exists(systemFile))
                {
                    string dirName = new DirectoryInfo(d).Name;
                    var systemData = SystemSingleton.Instance(dirName);
                    if (!systemData.IsPlugin) _systemList.Add(systemData); // Plugins are not systems
                }
            }
        }

        #region "base methods"

        public List<SystemLimpet> GetSystemList()
        {
            return _systemList;
        }
        public List<SystemLimpet> GetSystemActiveList()
        {
            var systemActiveList = new List<SystemLimpet>();
            foreach (var s in _systemList)
            {
                if (s.Active) systemActiveList.Add(s);
            }
            return systemActiveList;
        }
        public SystemLimpet GetSystemByKey(String key)
        {
            var ctrllist = from i in _systemList where i.Record.GetXmlProperty("genxml/systemkey") == key select i;
            if (ctrllist.Any()) return ctrllist.First(); 
            return null;
        }




        #endregion



    }

}
