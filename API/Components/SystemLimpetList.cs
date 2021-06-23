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
            var dl = Directory.GetDirectories(DNNrocketUtils.MapPath("/DesktopModules/DNNrocketModules"));
            foreach (var d in dl)
            {
                var systemFile = d + "\\system.rules";
                if (File.Exists(systemFile))
                {
                    string dirName = new DirectoryInfo(d).Name;
                    var systemData = new SystemLimpet(dirName);
                    _systemList.Add(systemData);
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
            var ctrllist = from i in _systemList where i.Record.GUIDKey == key select i;
            if (ctrllist.Any()) return ctrllist.First(); 
            return null;
        }




        #endregion



    }

}
