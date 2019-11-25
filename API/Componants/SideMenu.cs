using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Componants
{
    public class SideMenu
    {
        private SimplisityInfo _sysInfo;

        public string SystemKey { get; set; }
        public int SystemId { get; set; }
        public int ModuleId { get; set; }

        public SideMenu(SimplisityInfo sysInfo)
        {
            _sysInfo = sysInfo;
            if (sysInfo != null)
            {
                SystemKey = sysInfo.GetXmlProperty("genxml/textbox/ctrlkey");
            }
        }

        public List<SimplisityRecord> GetGroups()
        {
            var rtnList = new List<SimplisityRecord>();
            if (_sysInfo != null)
            {
                foreach (var i in _sysInfo.GetList("groupsdata"))
                {
                    // [TODO: add security]
                    rtnList.Add(i);
                }
            }

            return rtnList;
        }
        public List<SimplisityRecord> GetInterfaces(string groupref)
        {
            var rtnList = new List<SimplisityRecord>();

            if (_sysInfo != null)
            {
                foreach (var i in _sysInfo.GetList("interfacedata"))
                {
                    if (groupref == i.GetXmlProperty("genxml/dropdownlist/group"))
                    {
                        var userid = UserUtils.GetCurrentUserId();
                        var rocketinterface = new DNNrocketInterface(i);
                        if (rocketinterface.SecurityCheckUser(DNNrocketUtils.GetPortalId(), userid))
                        {
                            rtnList.Add(i);
                        }
                    }
                }
            }

            return rtnList;
        }

    }
}
