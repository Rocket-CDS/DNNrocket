using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Simplisity;

namespace DNNrocketAPI
{

    public class SystemDataList
    {
        private List<SimplisityInfo> _systemList;

        public SystemDataList()
        {
            _systemList = new List<SimplisityInfo>();
            var objCtrl = new DNNrocketController();
            var pluginfoldermappath = System.Web.Hosting.HostingEnvironment.MapPath("/DesktopModules/DNNrocket/api/Systems");
            if (pluginfoldermappath != null && Directory.Exists(pluginfoldermappath))
            {
                var upd = false;
                var flist = Directory.GetFiles(pluginfoldermappath, "*.xml");
                foreach (var f in flist)
                {
                    if (f.ToLower().EndsWith(".xml"))
                    {
                        var datain = File.ReadAllText(f);
                        try
                        {
                            var systemData = new SystemData("");
                            systemData.Import(datain);
                            upd = true;
                        }
                        catch (Exception)
                        {
                            // data might not be XML complient (ignore)
                        }
                        File.Delete(f);
                    }
                    if (upd)
                    {
                        CacheUtils.ClearAllCache();
                    }
                }
            }

            var l = objCtrl.GetList(-1, -1, "SYSTEM");
            foreach (var s in l)
            {
                _systemList.Add(s);
            }

        }

        #region "base methods"

        public List<SimplisityInfo> GetSystemList()
        {
            return _systemList;
        }

        public SimplisityInfo GetSystemByKey(String key)
        {
            var ctrllist = from i in _systemList where i.GUIDKey == key select i;
            if (ctrllist.Any()) return ctrllist.First(); 
            return null;
        }




        #endregion



    }

}
