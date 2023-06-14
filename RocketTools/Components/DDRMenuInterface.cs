using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DNNrocketAPI;
using DNNrocketAPI.Components;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Web.DDRMenu;
using Simplisity;

namespace RocketTools
{
    public class DdrMenuInterface : INodeManipulator
    {
        #region Implementation of INodeManipulator
        private DNNrocketController _objCtrl;

        public List<MenuNode> ManipulateNodes(List<MenuNode> nodes, PortalSettings portalSettings)
        {
            _objCtrl = new DNNrocketController();
            var settingRecord = _objCtrl.GetRecordByGuidKey(PortalUtils.GetPortalId(), -1, "PLSETTINGS", "PLSETTINGS");

            var nodeTabList = "*";
            foreach (var n in nodes)
            {
                nodeTabList += n.Text + n.TabId + "*" + n.Breadcrumb + "*" + n.Children.Count + "*";
            }

            var cachekey = "RocketTools*" + portalSettings.PortalId + "*" + DNNrocketUtils.GetCurrentCulture() + "*" + nodeTabList; // use nodeTablist incase the DDRMenu has a selector.
            List<MenuNode> rtnnodes = null;
            
            rtnnodes = (List<MenuNode>)CacheUtils.GetCache(cachekey);

            var debugMode = false;
            if (settingRecord != null)
            {
                debugMode = settingRecord.GetXmlPropertyBool("genxml/checkbox/debugmode");
            }
            if (rtnnodes != null && !debugMode)
            {
                return rtnnodes;
            }

            nodes = BuildNodes(nodes, portalSettings);

            if (settingRecord != null)
            {
                var menuproviders = settingRecord.GetRecordList("menuprovider");
                foreach (var p in menuproviders)
                {
                    var assembly = p.GetXmlProperty("genxml/textbox/assembly");
                    var namespaceclass = p.GetXmlProperty("genxml/textbox/namespaceclass");
                    var systemkey = p.GetXmlProperty("genxml/textbox/systemkey");
                    if (!String.IsNullOrEmpty(assembly) && !String.IsNullOrEmpty(namespaceclass))
                    {
                        var prov = MenuInterface.GetInstance(assembly,namespaceclass);
                        if (prov != null)
                        {
                            var tokenPrefix = prov.TokenPrefix();
                            // jump out if we don't have token in nodes
                            if (nodes.Count(x => x.Text.ToUpper().StartsWith(tokenPrefix.ToUpper())) == 0)
                            {
                                return nodes;
                            }
                            var idx = 0;
                            var idxlp = 0;
                            foreach (MenuNode n in nodes)
                            {
                                if (n.Depth == 0 && n.Text.ToUpper().StartsWith(tokenPrefix.ToUpper())) idx = idxlp;
                                idxlp += 1;
                            }
                            var nods = nodes.Where(x => x.Text.ToUpper().StartsWith(tokenPrefix.ToUpper())).ToList();
                            foreach (var n in nods)
                            {
                                var s = n.Text.Split(':');
                                var parentcatref = "";
                                if (s.Count() >= 2) parentcatref = s[1].TrimEnd(']');

                                var pageList = prov.GetMenuItems(PortalUtils.GetPortalId(), DNNrocketUtils.GetCurrentCulture(), systemkey, parentcatref);
                                var pageid = prov.PageId(PortalUtils.GetPortalId(), DNNrocketUtils.GetCurrentCulture());

                                nodes.Remove(n);

                                nodes = ProviderNodes(nodes, pageList, pageid, idx, 0);

                            }
                        }
                        else
                        {
                            LogUtils.LogSystem("ERROR: Invalid menu provider. " + assembly + "," + namespaceclass);
                        }
                    }
                }
            }

            // The nodes are passed by ref, so if they are manipulated after the nodes set has been passed back to DNN, the cache vallue will change. 
            CacheUtils.SetCache(cachekey, nodes);

            return nodes;
        }
        // ++++++ PROVIDER ++++++++++++++++
        private List<MenuNode> ProviderNodes(List<MenuNode> nodes, List<PageRecordData> pageList, int pageid, int idx, int parentId)
        {
            // Suports 4 levels.  The recursive code dropped the children.
            // I'm unsure why.  This is the quick fix and 4 levels should be OK.
            // This needs to be looked into if a recursive menu is required.

            var lp = idx;
            var queryPl = from pl in pageList where pl.ParentPageId == parentId select pl;
            foreach (var pg in queryPl)
            {
                var m = new MenuNode();
                var n2 = BuildMenuNode(pg, pageid, null);


                var queryPl3 = from pl in pageList where pl.ParentPageId == pg.PageId select pl;
                var childrenNodes = new List<MenuNode>();
                foreach (var pg3 in queryPl3)
                {
                    var n3 = BuildMenuNode(pg3, pageid, n2);

                    var queryPl4 = from pl in pageList where pl.ParentPageId == pg3.PageId select pl;
                    var childrenNodes4 = new List<MenuNode>();
                    foreach (var pg4 in queryPl4)
                    {
                        var n4 = BuildMenuNode(pg4, pageid, n3);

                        var queryPl5 = from pl in pageList where pl.ParentPageId == pg4.PageId select pl;
                        var childrenNodes5 = new List<MenuNode>();
                        foreach (var pg5 in queryPl5)
                        {
                            var n5 = BuildMenuNode(pg5, pageid, n4);
                            childrenNodes5.Add(n5);
                        }

                        n4.Children = childrenNodes5;
                        childrenNodes4.Add(n4);
                    }

                    n3.Children = childrenNodes4;
                    childrenNodes.Add(n3);
                }

                n2.Children = childrenNodes;
                nodes.Insert(lp, n2);

                lp += 1;
            }
            return nodes;
        }
        private MenuNode BuildMenuNode(PageRecordData pg, int pageid, MenuNode parentNode)
        {
            var n2 = new MenuNode();
            n2.TabId = pageid;
            n2.Text = pg.Name;
            n2.Url = pg.Url;
            n2.Keywords = pg.KeyWords;
            n2.Title = pg.Title;
            n2.Description = pg.Description;
            n2.Enabled = true;
            n2.Selected = false;
            n2.Breadcrumb = false;
            n2.Separator = false;
            if (parentNode != null)
            {
                parentNode.Children.Add(n2);
                n2.Parent = parentNode;
            }
            return n2;

        }

        // ++++++ BUILD ++++++++++++++++
        private List<MenuNode> BuildNodes(List<MenuNode> nodes, PortalSettings portalSettings, int depth = 0)
        {
            foreach (var n in nodes)
            {
                if (n.Depth == depth)
                {
                    var dataRecord = _objCtrl.GetRecordByGuidKey(portalSettings.PortalId, -1, "PL", "PL_" + DNNrocketUtils.GetCurrentCulture() + "_" + n.TabId.ToString(""));
                    if (dataRecord != null)
                    {
                        n.Text = dataRecord.GetXmlProperty("genxml/textbox/pagename");
                        n.Keywords = dataRecord.GetXmlProperty("genxml/textbox/tagwords");
                        n.Title = dataRecord.GetXmlProperty("genxml/textbox/pagetitle");
                        n.Description = dataRecord.GetXmlProperty("genxml/textbox/pagedescription");
                    }
                    if (n.Children.Count > 0) BuildNodes(n.Children, portalSettings, depth + 1);
                }
            }
            return nodes;
        }

        #endregion
    }


}
