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

        public SideMenu(SystemRecord sysRecord)
        {
            _sysRecord = sysRecord;
        }

        public List<SimplisityInfo> GetTopLevel()
        {
            var rtnList = new List<SimplisityInfo>();

            foreach (var i in _sysRecord.GetInterfaces())
            {

            }

            return rtnList;
        }

        public string GetSideMenu()
        {


            return "";
        }


    }
}
