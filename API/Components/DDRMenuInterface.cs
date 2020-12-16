using System;
using System.Collections.Generic;
using System.Web;
using DotNetNuke.Entities.Portals;
using Simplisity;
using DotNetNuke.Web.DDRMenu;


namespace DNNrocketAPI.Components
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


            nodes = BuildNodes(nodes, portalSettings);
            CacheUtilsDNN.SetCache(cachekey, nodes);
            return nodes;
        }



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
                        
                        if(dataRecord.GetXmlProperty("genxml/lang/genxml/textbox/pagename") != "")
                            n.Text = dataRecord.GetXmlProperty("genxml/lang/genxml/textbox/pagename");
                        if (dataRecord.GetXmlProperty("genxml/lang/genxml/textbox/tagwords") != "")
                            n.Keywords = dataRecord.GetXmlProperty("genxml/lang/genxml/textbox/tagwords");
                        if (dataRecord.GetXmlProperty("genxml/lang/genxml/textbox/pagetitle") != "")
                            n.Title = dataRecord.GetXmlProperty("genxml/lang/genxml/textbox/pagetitle");
                        if (dataRecord.GetXmlProperty("genxml/lang/genxml/textbox/description") != "")
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
