using System;
using System.Collections.Generic;
using System.Web;
using DotNetNuke.Entities.Portals;
using Simplisity;
using DotNetNuke.Web.DDRMenu;


namespace DNNrocketAPI.Componants
{
    public class DdrMenuInterface : INodeManipulator
    {
        #region Implementation of INodeManipulator
        private DNNrocketController _objCtrl;

        public List<MenuNode> ManipulateNodes(List<MenuNode> nodes, PortalSettings portalSettings)
        {
            _objCtrl = new DNNrocketController();
            //var settingRecord = _objCtrl.GetByGuidKey(portalSettings.PortalId, -1, "SETTINGS", "ROCKETPL");

            var nodeTabList = "*";
            foreach (var n in nodes)
            {
                nodeTabList += n.Text + n.TabId + "*" + n.Breadcrumb + "*";
            }
            var cachekey = "RocketPL*" + portalSettings.PortalId + "*" + DNNrocketUtils.GetCurrentCulture() + "*" + nodeTabList; // use nodeTablist incase the DDRMenu has a selector.
            //var rtnnodes = (List<MenuNode>)CacheUtils.GetCache(cachekey);

            //var debugMode = false;
            //if (settingRecord != null)
            //{
            //    debugMode = settingRecord.GetXmlPropertyBool("genxml/checkbox/debugmode");
            //}
            //if (rtnnodes != null && !debugMode) return rtnnodes;

            nodes = BuildNodes(nodes, portalSettings);

            //if (settingRecord != null)
            //{
            //    var menuproviders = settingRecord.GetXmlProperty("genxml/textbox/menuproviders");
            //    var provlist = menuproviders.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            //    foreach (var p in provlist)
            //    {
            //        var prov = CreateProvider(p);
            //        if (prov != null)
            //        {
            //            nodes = prov.ManipulateNodes(nodes, portalSettings);
            //        }
            //    }
            //}

            CacheUtils.SetCache(cachekey, nodes);
            return nodes;
        }

        //private static INodeManipulator CreateProvider(string providerAssembyClass)
        //{
        //    if (!string.IsNullOrEmpty(providerAssembyClass))
        //    {
        //        var prov = providerAssembyClass.Split(Convert.ToChar(","));
        //        try
        //        {
        //            var handle = Activator.CreateInstance(prov[1], prov[0]);
        //            return (INodeManipulator)handle.Unwrap();
        //        }
        //        catch (Exception ex)
        //        {
        //            // Error in provider is invalid provider, so remove from providerlist.
        //            return null;
        //        }
        //    }
        //    return null;
        //}

        private List<MenuNode> BuildNodes(List<MenuNode> nodes, PortalSettings portalSettings)
        {
            foreach (var n in nodes)
            {
                var guidKey = "tabid" + n.TabId;
                var dataRecord = _objCtrl.GetData(guidKey, "ROCKETPL", DNNrocketUtils.GetCurrentCulture());

                if (dataRecord != null)
                {
                    var dataRecordLang =  DNNrocketUtils.GetCurrentCulture();
                    if (dataRecordLang != null)
                    {
                        n.Text = dataRecord.GetXmlProperty("genxml/lang/genxml/textbox/pagename");
                        n.Keywords = dataRecord.GetXmlProperty("genxml/lang/genxml/textbox/tagwords");
                        n.Title = dataRecord.GetXmlProperty("genxml/lang/genxml/textbox/pagetitle");
                        n.Description = dataRecord.GetXmlProperty("genxml/lang/genxml/textbox/description");

                        if (n.Children.Count > 0) BuildNodes(n.Children, portalSettings);
                    }
                }
            }
            return nodes;
        }

        #endregion
    }

   
}
