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
        private List<SimplisityInfo> _systemList;

        public SystemLimpetList()
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
                            var systemData = new SystemLimpet("");
                            systemData.Import(datain);
                            upd = true;
                        }
                        catch (Exception ex)
                        {
                            LogUtils.LogException(ex);
                            // data might not be XML complient (ignore)
                        }
                        File.Delete(f);
                    }
                    if (upd)
                    {
                        CacheUtilsDNN.ClearAllCache();
                    }
                }
            }

            var l = objCtrl.GetList(-1, -1, "SYSTEM","","", " order by R1.SortOrder ");
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
