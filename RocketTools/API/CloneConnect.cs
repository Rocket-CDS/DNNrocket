using DNNrocketAPI.Components;
using Simplisity;
using System;
using RocketPortal.Components;
using System.Collections.Generic;
using System.Xml;
using RocketTools.Components;
using DotNetNuke.Entities.Modules;

namespace RocketTools.API
{
    public partial class StartConnect
    {

        public string CloneDetail()
        {
            var razorTempl = _appThemeTools.GetTemplate("Clones.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, null, null, null, null, true);
            if (!pr.IsValid) return pr.ErrorMsg;
            return pr.RenderedText;
        }

        public String GetCloneSelectModules()
        {
            var info = GetCachedInfo(_pageRef);
            info.GUIDKey = "";  // clear flag on new selection.

            info.RemoveRecordList("clonemodulelist");
            info.SetXmlProperty("genxml/fromtabid", _postInfo.GetXmlProperty("genxml/fromtabid"));
            var tabid = _postInfo.GetXmlPropertyInt("genxml/fromtabid");
            var pageData = new PageRecordData(_portalId, tabid);
            var l = RocketToolsUtils.GetTabModuleTitles(tabid);
            foreach (var m in l)
            {
                var modInfo = RocketToolsUtils.GetModuleInfo(tabid, m.Key);
                var sRec = new SimplisityRecord();
                sRec.SetXmlProperty("genxml/moduleid", m.Key.ToString());
                sRec.SetXmlProperty("genxml/modulename", m.Value);
                sRec.SetXmlProperty("genxml/pagename", pageData.Name);
                sRec.SetXmlProperty("genxml/modulepanename", modInfo.PaneName);
                info.AddRecordListItem("clonemodulelist", sRec);
            }
            CacheUtils.SetCache(_pageRef, info);
            var razorTempl = _appThemeTools.GetTemplate("clonesmodulesection.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, info, null, null, null, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }
        public String CloneDestination()
        {
            var info = GetCachedInfo(_pageRef);
            info.RemoveList("clonemoduleactive");
            foreach (SimplisityInfo m in info.GetList("clonemodulelist"))
            {
                if (_postInfo.GetXmlPropertyBool("genxml/checkbox/moduleid" + m.GetXmlProperty("genxml/moduleid")))
                {
                    if (info.GetListItem("clonemoduleactive", "genxml/moduleid", m.GetXmlProperty("genxml/moduleid")) == null) info.AddListItem("clonemoduleactive", m);
                }
            }
            CacheUtils.SetCache(_pageRef, info);
            var razorTempl = _appThemeTools.GetTemplate("clonesdestination.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, info, null, null, null, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }
        public void CloneModules()
        {
            var info = GetCachedInfo(_pageRef);
            var fromTabId = info.GetXmlPropertyInt("genxml/fromtabid");
            foreach (var m in info.GetRecordList("clonemodules"))
            {
                var moduleid = m.GetXmlPropertyInt("genxml/moduleid");
                if (moduleid > 0)
                {
                    foreach (var t in info.GetRecordList("clonetreeview"))
                    {
                        var toTabid = t.GetXmlPropertyInt("genxml/tabid");
                        var clone = t.GetXmlPropertyBool("genxml/clone");
                        CloneModule(moduleid, fromTabId, toTabid, clone);
                    }
                }
            }
        }
        public String ClonesOK()
        {
            var razorTempl = _appThemeTools.GetTemplate("clonesok.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, null, null, null, null, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }
        public void SaveModules(string listName)
        {
            var info = GetCachedInfo(_pageRef);
            info.GUIDKey = "";  // clear flag on new selection.

            var nodList = _postInfo.XMLDoc.SelectNodes("genxml/checkbox/*");
            info.RemoveRecordList(listName);
            foreach (XmlNode nod in nodList)
            {
                if (nod.InnerText.ToLower() == "true")
                {
                    var sRec = new SimplisityRecord();
                    sRec.SetXmlProperty("genxml/elementid", nod.Name);
                    sRec.SetXmlProperty("genxml/moduleid", nod.Name.Replace("moduleid", ""));
                    info.AddRecordListItem(listName, sRec);
                }
            }
            CacheUtils.SetCache(_pageRef, info);
        }
        public void SaveRoles(string listName)
        {
            var info = GetCachedInfo(_pageRef);
            info.GUIDKey = "";  // clear flag on new selection.

            var nodList = _postInfo.XMLDoc.SelectNodes("genxml/checkbox/*");
            info.RemoveRecordList(listName);
            foreach (XmlNode nod in nodList)
            {
                var sRec = new SimplisityRecord();
                sRec.SetXmlProperty("genxml/elementid", nod.Name);
                sRec.SetXmlProperty("genxml/roleid", nod.Name.Replace("roleid", ""));
                sRec.SetXmlProperty("genxml/addrole", nod.InnerText.ToLower());                    
                info.AddRecordListItem(listName, sRec);
            }
            CacheUtils.SetCache(_pageRef, info);
        }

        public void SaveTreeView(string listName)
        {
            var info = GetCachedInfo(_pageRef);
            info.GUIDKey = "";  // clear flag on new selection.

            var nodList = _postInfo.XMLDoc.SelectNodes("genxml/treeview/*/*");
            info.RemoveRecordList(listName);
            foreach (XmlNode nod in nodList)
            {
                var tabid = nod.Name.Replace("tabid", "");
                if (GeneralUtils.IsNumeric(tabid))
                {
                    var sRec = new SimplisityRecord();
                    sRec.SetXmlProperty("genxml/treeid", nod.Name);
                    sRec.SetXmlProperty("genxml/tabid", tabid);
                    var pageData = new PageRecordData(_portalId, Convert.ToInt32(tabid));
                    sRec.SetXmlProperty("genxml/tabname", pageData.Name);
                    sRec.SetXmlProperty("genxml/clone", nod.InnerText.ToLower());
                    info.AddRecordListItem(listName, sRec);
                }
            }
            CacheUtils.SetCache(_pageRef, info);
        }

        private void CloneModule(int moduleid, int fromTabId, int toTabId, bool clone)
        {
            if ((toTabId > 0) && (fromTabId > 0) && (fromTabId != toTabId))
            {
                var existingmodule = ModuleController.Instance.GetModule(moduleid, toTabId, true);
                if (existingmodule != null)
                {
                    if (existingmodule.IsDeleted || !clone)
                    {
                        ModuleController.Instance.DeleteTabModule(toTabId, moduleid, false);
                    }
                    existingmodule = ModuleController.Instance.GetModule(moduleid, toTabId, true);
                }
                if (existingmodule == null && clone)
                {
                    ModuleInfo fmi = ModuleController.Instance.GetModule(moduleid, fromTabId, true);
                    ModuleInfo newModule = fmi.Clone();

                    newModule.UniqueId = Guid.NewGuid(); // Cloned Module requires a different uniqueID 
                    newModule.TabID = toTabId;
                    ModuleController.Instance.AddModule(newModule);
                }

            }

        }

    }

}
