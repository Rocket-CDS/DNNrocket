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
        private SystemRecord _sysRecord;

        private string systemkey;

        public string SystemKey { get => systemkey; set => systemkey = value; }

        public SideMenu(SystemRecord sysRecord)
        {
            _sysRecord = sysRecord;
            systemkey = sysRecord.Info().GetXmlProperty("genxml/textbox/ctrlkey");
        }

        public List<SimplisityRecord> GetGroups()
        {
            var rtnList = new List<SimplisityRecord>();

            foreach (var i in _sysRecord.GetGroups())
            {
                // [TODO: add security]
                rtnList.Add(i);
            }

            return rtnList;
        }
        public List<SimplisityRecord> GetInterfaces(string groupref)
        {
            var rtnList = new List<SimplisityRecord>();

            foreach (var i in _sysRecord.GetInterfaces())
            {
                // [TODO: add security]
                if (groupref == i.GetXmlProperty("genxml/dropdownlist/group"))
                {
                    rtnList.Add(i);
                }
            }

            return rtnList;
        }

        public List<SimplisityRecord> GetMenuOnUserSecurity()
        {
            var roles = UserUtils.GetCurrentUserRoles();
            var rtnList = new List<SimplisityRecord>();
            foreach (var i in _sysRecord.GetInterfaces())
            {


            }

            return rtnList;
        }


    }
}
