using DNNrocketAPI.Components;
using Simplisity;
using System;
using RocketPortal.Components;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using RocketTools.Components;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;

namespace RocketTools.API
{
    public partial class StartConnect
    {
        public string PLDetail()
        {
            var info = _objCtrl.GetRecordByGuidKey(_portalId, -1, "PLSETTINGS", "PLSETTINGS");
            if (info == null) info = new SimplisityRecord();

            var razorTempl = _appThemeSystem.GetTemplate("pagelocalization.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, info, _dataObjects, null, _sessionParams, true);
            if (!pr.IsValid) return pr.ErrorMsg;
            return pr.RenderedText;
        }
        public string PLSettingsPopup()
        {
            var info = _objCtrl.GetRecordByGuidKey(_portalId, -1, "PLSETTINGS", "PLSETTINGS");
            if (info == null) info = new SimplisityRecord();

            var razorTempl = _appThemeSystem.GetTemplate("SettingsPopup.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, info, _dataObjects, null, _sessionParams, true);
            if (!pr.IsValid) return pr.ErrorMsg;
            return pr.RenderedText;
        }
        public string PageView()
        {
            var tabid = _postInfo.GetXmlPropertyInt("genxml/selectedtabid");
            var pageData = new PageRecordData(_portalId, Convert.ToInt32(tabid));
            _dataObjects.Add("pagedata", pageData);
            var selectedCulture = _paramInfo.GetXmlProperty("genxml/hidden/culturecode1");
            if (selectedCulture == "")
            {
                selectedCulture = DNNrocketUtils.GetCurrentCulture();
                _sessionParams.Set("culturecode1", selectedCulture);
            }
            var info = _objCtrl.GetRecordByGuidKey(_portalId, -1, "PL", "PL_" + selectedCulture + "_" + pageData.Record.ItemID);
            if (info == null)
            {
                info = new SimplisityRecord();
                info.SetXmlProperty("genxml/textbox/pagename", pageData.Name);
                info.SetXmlProperty("genxml/textbox/pagetitle", pageData.Title);
                info.SetXmlProperty("genxml/textbox/tagwords", pageData.KeyWords);
                info.SetXmlProperty("genxml/textbox/pagedescription", pageData.Description);
                info.SetXmlProperty("genxml/textbox/pageurl", pageData.Url);
            }
            var razorTempl = _appThemeSystem.GetTemplate("pageview.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, info, _dataObjects, null, _sessionParams, true);
            if (!pr.IsValid) return pr.ErrorMsg;
            return pr.RenderedText;
        }
        public string PageEdit()
        {
            var tabid = _postInfo.GetXmlPropertyInt("genxml/selectedtabid");
            var pageData = new PageRecordData(_portalId, Convert.ToInt32(tabid));
            _dataObjects.Add("pagedata", pageData);
            var selectedCulture = _paramInfo.GetXmlProperty("genxml/hidden/culturecode2");
            if (selectedCulture == "")
            {
                selectedCulture = DNNrocketUtils.GetCurrentCulture();
                _sessionParams.Set("culturecode2", selectedCulture);
            }
            var info = _objCtrl.GetRecordByGuidKey(_portalId, -1, "PL", "PL_" + selectedCulture + "_" + tabid);
            if (info == null)
            {
                info = new SimplisityRecord();
                info.SetXmlProperty("genxml/textbox/pagename", pageData.Name);
                info.SetXmlProperty("genxml/textbox/pagetitle", pageData.Title);
                info.SetXmlProperty("genxml/textbox/tagwords", pageData.KeyWords);
                info.SetXmlProperty("genxml/textbox/pagedescription", pageData.Description);
                info.SetXmlProperty("genxml/textbox/pageurl", pageData.Url);
            }
            var razorTempl = _appThemeSystem.GetTemplate("pageedit.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, info, _dataObjects, null, _sessionParams, true);
            if (!pr.IsValid) return pr.ErrorMsg;
            return pr.RenderedText;
        }
        public string PageSave()
        {
            var tabid = _postInfo.GetXmlPropertyInt("genxml/selectedtabid");
            var selectedCulture = _paramInfo.GetXmlProperty("genxml/hidden/culturecode2");
            if (selectedCulture == "")
            {
                selectedCulture = DNNrocketUtils.GetCurrentCulture();
                _sessionParams.Set("culturecode2", selectedCulture);
            }
            var guidkey = "PL_" + selectedCulture + "_" + tabid;
            var info = _objCtrl.GetRecordByGuidKey(_portalId, -1, "PL", guidkey);
            if (info == null)
            {
                info = new SimplisityRecord();
                info.ItemID = -1;
                info.PortalId = _portalId;
                info.TypeCode = "PL";
                info.GUIDKey = guidkey;
                info.SortOrder = tabid;
            }
            info.Lang = selectedCulture;

            _postInfo.RemoveXmlNode("genxml/hidden");
            _postInfo.RemoveXmlNode("genxml/lang");
            info.XMLData = _postInfo.XMLData;
            info.ItemID = _objCtrl.Update(info);

            // reload TabData, so we can save, clear cache and validate (reformat url)
            var tabD = new TabData(info);
            tabD.Save();

            // Update Meta data for DNN Tab DB, if the meta is empty DNN will not render the meta data.
            var controller = new TabController();
            var newTab = controller.GetTab(tabD.TabId, _portalId);
            if (newTab.Description == "") newTab.Description = ".";
            if (newTab.KeyWords == "") newTab.KeyWords = ".";
            controller.UpdateTab(newTab);

            SaveTabUrls(tabD);

            return "OK";
        }
        public static void SaveTabUrls(TabData tabData)
        {
            try
            {
                if (tabData != null)
                {

                    //save data
                    if (tabData.DataRecord.ItemID > 0)
                    {
                        // update tab url table.
                        if (tabData.PageUrl != "")
                        {
                            RocketToolsUtils.ValidateTabUrls(tabData);

                            var pageurl = tabData.PageUrl;
                            if (!pageurl.StartsWith("/")) pageurl = "/" + pageurl;

                            var objTabs = new TabController();
                            var tabUrlList = objTabs.GetTabUrls(tabData.TabId, PortalSettings.Current.PortalId);
                            TabUrlInfo tabUrlInfo = null;
                            foreach (var t in tabUrlList)
                            {
                                // Get matching langauge.
                                if (t.CultureCode == tabData.DataRecord.Lang && t.HttpStatus == "200")
                                {
                                    tabUrlInfo = t;
                                }
                            }
                            if (tabUrlInfo == null)
                            {
                                tabUrlInfo = new TabUrlInfo();
                                tabUrlInfo.SeqNum = tabUrlList.Count + 1;
                            }
                            if (tabUrlInfo.Url != pageurl)
                            {
                                // create to 301.
                                // Who decided that saving a taburl would trigger the creation of a 301????
                                // This is complicated and confusing for people using it.  
                                // If you add an existing 301 url, it causes a duplicate for 200 and 301.
                                tabUrlInfo.SeqNum = tabUrlList.Count + 1;
                            }

                            tabUrlInfo.TabId = tabData.TabId;
                            tabUrlInfo.HttpStatus = "200";
                            tabUrlInfo.Url = pageurl;
                            tabUrlInfo.QueryString = "";
                            tabUrlInfo.CultureCode = tabData.DataRecord.Lang;
                            tabUrlInfo.IsSystem = true;
                            tabUrlInfo.PortalAliasUsage = 0;
                            objTabs.SaveTabUrl(tabUrlInfo, PortalSettings.Current.PortalId, true);

                            RocketToolsUtils.ValidateTabUrls(tabData);
                            RocketToolsUtils.ValidateTabUrls(tabData); // redo, catch ALL duplicates after first validate seqnum change.

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogUtils.LogException(ex);
            }

        }

        public string PageDelete()
        {
            var tabid = _postInfo.GetXmlPropertyInt("genxml/selectedtabid");
            var selectedCulture = _paramInfo.GetXmlProperty("genxml/hidden/culturecode2");
            if (selectedCulture == "")
            {
                selectedCulture = DNNrocketUtils.GetCurrentCulture();
                _sessionParams.Set("culturecode2", selectedCulture);
            }
            var guidkey = "PL_" + selectedCulture + "_" + tabid;
            var info = _objCtrl.GetRecordByGuidKey(_portalId, -1, "PL", guidkey);
            if (info != null) _objCtrl.Delete(info.ItemID);
            return "OK";
        }

        public string SaveSettings()
        {
            var info = _objCtrl.GetRecordByGuidKey(_portalId, -1, "PLSETTINGS", "PLSETTINGS");
            if (info == null)
            {
                info = new SimplisityRecord();
                info.ItemID = -1;
                info.PortalId = _portalId;
                info.TypeCode = "PLSETTINGS";
                info.GUIDKey = "PLSETTINGS";
            }
            _postInfo.RemoveXmlNode("genxml/hidden");
            _postInfo.RemoveXmlNode("genxml/lang");
            info.XMLData = _postInfo.XMLData;
            _objCtrl.Update(info);

            CacheUtils.ClearAllCache(_portalId.ToString());

            return PLDetail();
        }

        public void DeleteTabUrl()
        {
            var tabid = _paramInfo.GetXmlPropertyInt("genxml/hidden/tabid");
            var seqnum = _paramInfo.GetXmlPropertyInt("genxml/hidden/seqnum");
            RocketToolsUtils.DeleteTabUrl(_portalId, tabid, seqnum);
        }
        public string AddMenuProvider()
        {
            var info = _objCtrl.GetRecordByGuidKey(_portalId, -1, "PLSETTINGS", "PLSETTINGS");
            if (info == null)
            {
                SaveSettings();
                info = _objCtrl.GetRecordByGuidKey(_portalId, -1, "PLSETTINGS", "PLSETTINGS");
            }
            if (info != null)
            {
                info.AddListItem("menuprovider", "<genxml></genxml>");
                _objCtrl.Update(info);
            }
            return PLSettingsPopup();
        }
        public string AddQueryParam()
        {
            var info = _objCtrl.GetRecordByGuidKey(_portalId, -1, "PLSETTINGS", "PLSETTINGS");
            if (info == null)
            {
                SaveSettings();
                info = _objCtrl.GetRecordByGuidKey(_portalId, -1, "PLSETTINGS", "PLSETTINGS");
            }
            if (info != null)
            {
                info.AddListItem("queryparams", "<genxml></genxml>");
                _objCtrl.Update(info);
            }
            return PLSettingsPopup();
        }

    }

}
