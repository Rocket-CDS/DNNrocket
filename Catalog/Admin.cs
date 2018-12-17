using DNNrocketAPI;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocket.Catalog
{
    public class Admin
    {

        public string ProductAdminList(SimplisityInfo ajaxInfo, string editlang, string entityTypeCode, bool paging = true, SimplisityInfo ajaxHeaderInfo = null)
        {
            try
            {
                    if (UserController.Instance.GetCurrentUserInfo().UserID <= 0) return null;

                    var strOut = "";

                var entityTypeCodeLang = entityTypeCode + "LANG";

                    var filter = ajaxInfo.GetXmlProperty("genxml/hidden/filter");
                    var orderby = ajaxInfo.GetXmlProperty("genxml/hidden/orderby");
                    var returnLimit = ajaxInfo.GetXmlPropertyInt("genxml/hidden/returnlimit");
                    var pageNumber = ajaxInfo.GetXmlPropertyInt("genxml/hidden/pagenumber");
                    var pageSize = ajaxInfo.GetXmlPropertyInt("genxml/hidden/pagesize");
                    var cascade = ajaxInfo.GetXmlPropertyBool("genxml/hidden/cascade");
                    var portalId = PortalSettings.Current.PortalId;
                    var searchText = ajaxInfo.GetXmlProperty("genxml/hidden/searchtext");
                    var searchhidden = ajaxInfo.GetXmlProperty("genxml/hidden/searchhidden");
                    var searchvisible = ajaxInfo.GetXmlProperty("genxml/hidden/searchvisible");
                    var searchenabled = ajaxInfo.GetXmlProperty("genxml/hidden/searchenabled");
                    var searchdisabled = ajaxInfo.GetXmlProperty("genxml/hidden/searchdisabled");

                    // ---------- search category/property list ----------------------------
                    var filterCatList = "(";
                    var searchCategory = ajaxInfo.GetXmlProperty("genxml/hidden/searchcategory");
                    var searchProperty = ajaxInfo.GetXmlProperty("genxml/hidden/searchproperty");
                    var defcatlistsplit = (searchCategory + searchProperty).Split(',');
                    var clp = 0;
                    foreach (var c in defcatlistsplit)
                    {
                    if (GeneralUtils.IsNumeric(c) && Convert.ToInt32(c) > 0)
                        {
                            filterCatList += " XrefItemId = " + c + " ";
                            filterCatList += "|"; // use | so we can trim replace easy.
                            clp += 1;
                        }
                    }
                    filterCatList = filterCatList.TrimEnd('|');
                    filterCatList = filterCatList.Replace("|", " or ");
                    filterCatList += ")";
                    // ---------------------------------------------------------------------


                    if (searchText != "") filter += " and (NB3.[ProductName] like '%" + searchText + "%' or NB3.[ProductRef] like '%" + searchText + "%' or NB3.[Summary] like '%" + searchText + "%' ) ";

                    if (searchCategory != "" || searchProperty != "")
                    {
                        if (clp == 1)
                        {
                            if (orderby == "{bycategoryproduct}") orderby += searchCategory.Trim(',') + searchProperty.Trim(',');
                        }
                        else
                        {
                            if (orderby == "{bycategoryproduct}") orderby = " order by NB3.productname ";
                        }

                        var objQual = DotNetNuke.Data.DataProvider.Instance().ObjectQualifier;
                        var dbOwner = DotNetNuke.Data.DataProvider.Instance().DatabaseOwner;
                        if (!cascade)
                            filter += " and NB1.[ItemId] in (select parentitemid from " + dbOwner + "[" + objQual + "NBrightBuy] where typecode = 'CATXREF' and " + filterCatList + ") ";
                        else
                            filter += " and NB1.[ItemId] in (select parentitemid from " + dbOwner + "[" + objQual + "NBrightBuy] where (typecode = 'CATCASCADE' or typecode = 'CATXREF') and " + filterCatList + ") ";

                    }
                    else
                    {
                        if (orderby == "{bycategoryproduct}") orderby = " order by NB3.productname ";
                    }

                    // logic for client list of products
                    if (DNNrocketUtils.IsClientOnly())
                    {
                        filter += " and NB1.ItemId in (select ParentItemId from dbo.[NBrightBuy] as NBclient where NBclient.TypeCode = 'USERPRDXREF' and NBclient.UserId = " + UserController.Instance.GetCurrentUserInfo().UserID.ToString("") + ") ";
                    }

                    // get any plugin data records.
                    var plugindatasql = " and (NB1.TypeCode = '" + entityTypeCode + "'";


                    filter = plugindatasql + ") " + filter;

                    // --- Hidden or Visible 
                    // Both hidden and visible selected, don't do any SQL filter so we pick up all.
                    if ((searchhidden != "" && searchhidden.ToLower() != "true") && (searchvisible != "" && searchvisible.ToLower() != "true"))
                    {
                        filter += " and (NB3.Visible = 0) and (NB3.Visible = 1)  "; // don't display anything!!!
                    }
                    if ((searchhidden != "" && searchhidden.ToLower() == "true") && (searchvisible != "" && searchvisible.ToLower() != "true"))
                    {
                        filter += " and (NB3.Visible = 0) "; // display hidden
                    }
                    if ((searchhidden != "" && searchhidden.ToLower() != "true") && (searchvisible != "" && searchvisible.ToLower() == "true"))
                    {
                        filter += " and (NB3.Visible = 1)  "; // display visible
                    }

                    // --- Enabled or Disabled
                    // Both Enabled and Disabled selected, don't do any SQL filter so we pick up all.
                    if ((searchenabled != "" && searchenabled.ToLower() != "true") && (searchdisabled != "" && searchdisabled.ToLower() != "true"))
                    {
                        filter += " and (NB1.XMLData.value('(genxml/checkbox/chkdisable)[1]','nvarchar(5)') = 'False') and (NB1.XMLData.value('(genxml/checkbox/chkdisable)[1]','nvarchar(5)') = 'True')  "; // don't display anything!!!
                    }
                    if ((searchenabled != "" && searchenabled.ToLower() == "true") && (searchdisabled != "" && searchdisabled.ToLower() != "true"))
                    {
                        filter += " and (NB1.XMLData.value('(genxml/checkbox/chkdisable)[1]','nvarchar(5)') = 'False') "; // display enabled
                    }
                    if ((searchenabled != "" && searchenabled.ToLower() != "true") && (searchdisabled != "" && searchdisabled.ToLower() == "true"))
                    {
                        filter += " and (NB1.XMLData.value('(genxml/checkbox/chkdisable)[1]','nvarchar(5)') = 'True')  "; // display disabled
                    }


                    var recordCount = 0;
                    var objCtrl = new DNNrocketController();

                    if (paging) // get record count for paging
                    {
                        if (pageNumber == 0) pageNumber = 1;
                        if (pageSize == 0) pageSize = 20;

                        // get only entity type required.  Do NOT use typecode, that is set by the filter.
                        recordCount = objCtrl.GetListCount(PortalSettings.Current.PortalId, -1, "", filter, "", EditLangCurrent);

                    }

                    // get selected entitytypecode.
                    var list = objCtrl.GetDataList(PortalSettings.Current.PortalId, -1, "", "", EditLangCurrent, filter, orderby, StoreSettings.Current.DebugMode, "", returnLimit, pageNumber, pageSize, recordCount);

                    return RenderProductAdminList(list, ajaxInfo, recordCount, ajaxHeaderInfo);

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public String RenderProductAdminList(List<SimplisityInfo> list, SimplisityInfo ajaxInfo, int recordCount)
        {
            return RenderProductAdminList(list, ajaxInfo, recordCount, null);
        }

        public String RenderProductAdminList(List<SimplisityInfo> list, SimplisityInfo ajaxInfo, int recordCount, SimplisityInfo headerDataInfo)
        {

            try
            {
                    if (headerDataInfo == null)
                    {
                        headerDataInfo = ajaxInfo;
                    }


                    if (list == null) return "";
                    if (UserController.Instance.GetCurrentUserInfo().UserID <= 0) return "";

                    if (EditLangCurrent == "") EditLangCurrent = Utils.GetCurrentCulture();

                    var strOut = "";

                    // select a specific entity data type for the product (used by plugins)
                    var themeFolder = ThemeFolder;
                    if (themeFolder == "") themeFolder = ajaxInfo.GetXmlProperty("genxml/hidden/themefolder");
                    if (themeFolder == "") themeFolder = ThemeFolder;
                    var pageNumber = ajaxInfo.GetXmlPropertyInt("genxml/hidden/pagenumber");
                    var pageSize = ajaxInfo.GetXmlPropertyInt("genxml/hidden/pagesize");
                    if (RazorTemplate == "") RazorTemplate = ajaxInfo.GetXmlProperty("genxml/hidden/razortemplate");

                    bool paging = pageSize > 0;

                    var passSettings = new Dictionary<string, string>();
                    foreach (var s in ajaxInfo.ToDictionary())
                    {
                        passSettings.Add(s.Key, s.Value);
                    }
                    foreach (var s in StoreSettings.Current.Settings()) // copy store setting, otherwise we get a byRef assignement
                    {
                        if (passSettings.ContainsKey(s.Key))
                            passSettings[s.Key] = s.Value;
                        else
                            passSettings.Add(s.Key, s.Value);
                    }

                    strOut = NBrightBuyUtils.RazorTemplRenderList(RazorTemplate, 0, "", list, TemplateRelPath, themeFolder, EditLangCurrent, passSettings, headerDataInfo);

                    // add paging if needed
                    if (paging && (recordCount > pageSize))
                    {
                        var pg = new NBrightCore.controls.PagingCtrl();
                        strOut += pg.RenderPager(recordCount, pageSize, pageNumber);
                    }

                    return strOut;

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }


    }
}
