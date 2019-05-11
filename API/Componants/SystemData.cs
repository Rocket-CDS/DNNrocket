using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Simplisity;

namespace DNNrocketAPI
{

    public class SystemData
    {
        private List<SimplisityInfo> _systemList;

        public SystemData()
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
                            var nbi = new SimplisityInfo();
                            nbi.XMLData = datain;
                            // check if we are injecting multiple
                            var nodlist = nbi.XMLDoc.SelectNodes("genxml");
                            if (nodlist != null && nodlist.Count > 0)
                            {
                                foreach (XmlNode nod in nodlist)
                                {
                                    var nbi2 = new SimplisityInfo();
                                    nbi2.XMLData = nod.OuterXml;
                                    nbi2.ItemID = -1;
                                    nbi2.GUIDKey = nbi.GetXmlProperty("genxml/textbox/ctrlkey");
                                    nbi2.PortalId = 99999;
                                    nbi2.Lang = "";
                                    nbi2.ParentItemId = 0;
                                    nbi2.ModuleId = -1;
                                    nbi2.XrefItemId = 0;
                                    nbi2.UserId = 0;
                                    nbi2.TypeCode = "SYSTEM";

                                    var s = objCtrl.GetByGuidKey(-1, -1, "SYSTEM", nbi2.GUIDKey);

                                    if (s != null) nbi2.ItemID = s.ItemID;

                                    objCtrl.Update(nbi2);
                                    upd = true;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            // data might not be XML complient (ignore)
                        }
                        File.Delete(f);
                    }
                    if (upd)
                    {
                        CacheUtils.ClearCache();
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
