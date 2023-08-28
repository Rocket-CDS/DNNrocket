using DNNrocketAPI;
using DNNrocketAPI.Components;
using DotNetNuke.Entities.Tabs;
using RazorEngine.Text;
using RocketTools.Components;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace RocketTools
{
    public class RocketToolsTokens<T> : DNNrocketAPI.render.DNNrocketTokens<T>
    {
        public IEncodedString GetTabList(int portalId, List<int> selectedTabIdList, string treeviewId, string lang = "", string attributes = "", bool showAllTabs = false)
        {
            if (lang == "") lang = DNNrocketUtils.GetCurrentCulture();
            var tabList = TabController.GetTabsBySortOrder(portalId, lang, true);
            var rtnString = "";
            var strOut = GetTabList(rtnString, tabList, 0, 0, treeviewId, attributes, selectedTabIdList, showAllTabs);
            strOut += "";
            return new RawString(strOut);
        }
        private static string GetTabList(string rtnString, List<TabInfo> tabList, int level, int parentid, string id, string attributes, List<int> selectedTabIdList, bool showAllTabs)
        {

            if (level > 50) // stop infinate loop
            {
                return rtnString;
            }

            foreach (TabInfo tInfo in tabList)
            {
                if (!tInfo.IsDeleted)
                {
                    var parenttestid = tInfo.ParentId;
                    if (parenttestid < 0) parenttestid = 0;
                    if (parentid == parenttestid)
                    {
                        var checkedvalue = "";
                        if (selectedTabIdList.Contains(tInfo.TabID)) checkedvalue = "checked";

                        rtnString += "<div>";
                        for (int i = 0; i < level; i++)
                        {
                            rtnString += "&nbsp;&nbsp;";
                        }
                        rtnString += "<input id='tabid-" + id + "-" + tInfo.TabID + "' data-id='" + tInfo.TabID + "' s-xpath='genxml/treeview/" + id + "/tabid" + tInfo.TabID + "' s-update='save' " + checkedvalue + " " + attributes + " type='checkbox'>";
                        rtnString += "&nbsp;" + tInfo.TabName;
                        rtnString += "</div>";
                        if (tInfo.HasChildren)
                        {
                            rtnString = GetTabList(rtnString, tabList, level + 1, tInfo.TabID, id, attributes, selectedTabIdList, showAllTabs);
                        }
                    }
                }
            }
            return rtnString;
        }


    }
}
