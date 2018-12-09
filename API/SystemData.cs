using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Web;
using System.Web.Handlers;
using System.Web.UI.WebControls;
using System.Xml;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.UI.UserControls;
using DotNetNuke.UI.WebControls;
using NBrightCore.common;
using NBrightCore.render;
using NBrightDNN;
using Nevoweb.DNN.NBrightBuy.Components;
using Nevoweb.DNN.NBrightBuy.Components.Interfaces;
using Simplisity;

namespace DNNrocketAPI
{

    public class SystemData
    {
        private List<SimplisityInfo> _systemList;

        public SystemData()
        {
            _systemList = new List<SimplisityInfo>();
            var pluginfoldermappath = System.Web.Hosting.HostingEnvironment.MapPath("/DesktopModules/DNNrocket/api/Systems");
            if (pluginfoldermappath != null && Directory.Exists(pluginfoldermappath))
            {
                var objCtrl = new NBrightBuyController();
                var flist = Directory.GetFiles(pluginfoldermappath, "*.xml");
                foreach (var f in flist)
                {
                    if (f.ToLower().EndsWith(".xml"))
                    {
                        var datain = File.ReadAllText(f);
                        try
                        {
                            var nbi = new NBrightInfo();
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
                                    nbi2.GUIDKey = nbi.GetXmlProperty("genxml/textbox/ctrl");
                                    nbi2.PortalId = 99999;
                                    nbi2.Lang = "";
                                    nbi2.ParentItemId = 0;
                                    nbi2.ModuleId = -1;
                                    nbi2.XrefItemId = 0;
                                    nbi2.UserId = 0;
                                    nbi2.TypeCode = "SYSTEM";

                                    _systemList.Add(nbi2);

                                }
                            }
                        }
                        catch (Exception)
                        {
                            // data might not be XML complient (ignore)
                        }
                    }
                }
            }
        }

        #region "base methods"

        public List<SimplisityInfo> GetSystemList()
        {
            return _systemList;
        }

        public SimplisityInfo GetSystemByKey(String key)
        {
            var ctrllist = from i in _systemList where i.GetXmlProperty("genxml/textbox/ctrl") == key select i;
            if (ctrllist.Any()) return ctrllist.First(); 
            return null;
        }

        #endregion



    }
}
