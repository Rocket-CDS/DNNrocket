using System;
using System.Collections.Generic;
using DNNrocket.Login;
using DNNrocketAPI;
using Simplisity;

namespace RocketMod
{
    public class startconnect : DNNrocketAPI.APInterface
    {
        private static string _appthemeRelPath;
        private static string _appthemeMapPath;
        public override Dictionary<string, string> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, string userHostAddress, string editlang = "")
        {
            var strOut = "";
            _appthemeRelPath = "/DesktopModules/DNNrocket/AppThemes";
            _appthemeMapPath = DNNrocketUtils.MapPath(_appthemeRelPath);

            var rocketInterface = new DNNrocketInterface(interfaceInfo);

            // we should ALWAYS pass back the moduleid in the template post.
            // But for the admin start we need it to be passed by the admin.aspx url parameters.  Which then puts it in the s-fields for the simplsity start call.
            var moduleid = postInfo.GetXmlPropertyInt("genxml/hidden/moduleid");
            if (moduleid == 0) moduleid = postInfo.ModuleId;
            if (moduleid == 0)
            {
                strOut = "ERROR: No moduleId has been passed to the API";
            }
            else
            {

                // use command form cookie if we have set it.
                var cookieCmd = DNNrocketUtils.GetCookieValue("rocketmod_cmd");
                if (cookieCmd != "")
                {
                    paramCmd = cookieCmd;
                    DNNrocketUtils.DeleteCookieValue("rocketmod_cmd");
                }

                if (DNNrocketUtils.SecurityCheckCurrentUser(rocketInterface))
                {
                    switch (paramCmd)
                    {
                        case "rocketmod_edit":
                            strOut = GetEditData(moduleid, rocketInterface, postInfo);
                            break;
                        case "rocketmod_savedata":
                            strOut = GetSaveData(moduleid, rocketInterface, postInfo);
                            break;
                        case "rocketmod_delete":
                            DeleteData(moduleid, postInfo);
                            strOut = GetEditData(moduleid, rocketInterface, postInfo);
                            break;
                        case "rocketmod_saveconfig":
                            postInfo.SetXmlProperty("genxml/appthemerelpath", _appthemeRelPath); // assign the template path to check if a list or not.
                            strOut = ConfigUtils.SaveConfig(moduleid, postInfo);
                            if (strOut == "")
                            {
                                // not error returned , so return Dashboard.
                                strOut = GetDashBoard(moduleid, rocketInterface);
                            }
                            break;
                        case "rocketmod_getsetupmenu":
                            strOut = ConfigUtils.GetSetup(moduleid, rocketInterface);
                            break;
                        case "rocketmod_dashboard":
                            strOut = GetDashBoard(moduleid, rocketInterface);
                            break;
                        case "rocketmod_reset":
                            strOut = ResetRocketMod(moduleid, rocketInterface);
                            break;
                    }
                }
            }
            switch (paramCmd)
            {
                case "rocketmod_login":
                    strOut = LoginUtils.DoLogin(postInfo, userHostAddress);
                    break;
                case "rocketmod_getdata":
                    strOut = GetData(moduleid, rocketInterface, postInfo);
                    break;
                case "rocketmod_adminurl":
                    strOut = "/desktopmodules/dnnrocket/RocketMod/admin.aspx";
                    break;
            }

            if (strOut == "")
            {
                postInfo.SetXmlProperty("genxml/interfacekey", rocketInterface.InterfaceKey);
                strOut = LoginUtils.LoginForm(postInfo);
            }

            var rtnDic = new Dictionary<string, string>();
            rtnDic.Add("outputhtml", strOut);
            return rtnDic;
        }

        public static void DeleteData(int moduleid, SimplisityInfo postInfo)
        {
            var selecteditemid = postInfo.GetXmlPropertyInt("genxml/hidden/selecteditemid");
            var objCtrl = new DNNrocketController();
            objCtrl.Delete(selecteditemid);
        }

        public static String GetEditData(int moduleid, DNNrocketInterface rocketInterface, SimplisityInfo postInfo)
        {
            try
            {
                var strOut = "";
                var themeFolder = "";
                var razortemplate = "";
                var objCtrl = new DNNrocketController();
                var configInfo = objCtrl.GetData("moduleconfig", "CONFIG", DNNrocketUtils.GetEditCulture(), -1, moduleid);
                if (configInfo.GetXmlProperty("genxml/checkbox/datalist") == "")
                {
                    // no display type set, return dashboard.
                    return GetDashBoard(moduleid, rocketInterface);
                }

                if (configInfo.GetXmlPropertyBool("genxml/checkbox/datalist"))
                {
                    razortemplate = "editlist.cshtml";
                    var info = objCtrl.GetData(moduleid.ToString(), rocketInterface.EntityTypeCode, DNNrocketUtils.GetEditCulture(),-1, moduleid);
                    themeFolder = configInfo.GetXmlProperty("genxml/select/apptheme");
                    postInfo.ModuleId = moduleid; // make sure we have correct moduleid.
                    strOut = GetList(rocketInterface, postInfo, _appthemeRelPath, razortemplate, themeFolder);
                }
                else
                {
                    razortemplate = "edit.cshtml";
                    var info = objCtrl.GetData(moduleid.ToString(), rocketInterface.EntityTypeCode, DNNrocketUtils.GetEditCulture(),-1, moduleid);
                    themeFolder = configInfo.GetXmlProperty("genxml/select/apptheme");
                    rocketInterface.ModuleId = moduleid;  // set the moduleid so it's set to the Model.ModuleId value
                    var passSettings = rocketInterface.ToDictionary();
                    var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, _appthemeRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());
                    passSettings.Add("mappathAppThemeFolder", _appthemeMapPath);
                    strOut = DNNrocketUtils.RazorDetail(razorTempl, info, passSettings);
                }

                if (strOut == "") strOut = "ERROR: No data returned for " + _appthemeMapPath + "\\" + themeFolder + "\\default\\" + razortemplate;
                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        private static String GetList(DNNrocketInterface rocketInterface, SimplisityInfo postInfo, string templateControlRelPath, string razortemplate,string themeFolder)
        {
            try
            {

                var page = postInfo.GetXmlPropertyInt("genxml/hidden/page");
                var pagesize = postInfo.GetXmlPropertyInt("genxml/hidden/pagesize");

                var searchtext = postInfo.GetXmlProperty("genxml/textbox/searchtext");

                var filter = "";
                if (searchtext != "")
                {
                    filter = " and searchfield.GuidKey like '%" + searchtext + "%'";
                }


                var objCtrl = new DNNrocketController();
                var listcount = objCtrl.GetListCount(postInfo.PortalId, postInfo.ModuleId, rocketInterface.EntityTypeCode, filter, DNNrocketUtils.GetEditCulture());
                var list = objCtrl.GetList(postInfo.PortalId, postInfo.ModuleId, rocketInterface.EntityTypeCode, filter, DNNrocketUtils.GetEditCulture(), "", 0, page, pagesize, listcount);

                var headerData = new SimplisityInfo();
                headerData.SetXmlProperty("genxml/hidden/rowcount", listcount.ToString());
                headerData.SetXmlProperty("genxml/hidden/page", page.ToString());
                headerData.SetXmlProperty("genxml/hidden/pagesize", pagesize.ToString());
                headerData.SetXmlProperty("genxml/textbox/searchtext", searchtext);

                return RenderList(list, postInfo, 0, templateControlRelPath, headerData, razortemplate, themeFolder);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        private static String RenderList(List<SimplisityInfo> list, SimplisityInfo sInfo, int recordCount, string templateControlRelPath, SimplisityInfo headerData, string razortemplate, string themeFolder)
        {
            try
            {
                if (list == null) return "";
                var strOut = "";
                var passSettings = sInfo.ToDictionary();
                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());
                strOut = DNNrocketUtils.RazorList(razorTempl, list, passSettings, headerData);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        public static String GetSaveData(int moduleid, DNNrocketInterface rocketInterface, SimplisityInfo postInfo)
        {
            try
            {
                var objCtrl = new DNNrocketController();
                var info = objCtrl.GetData("moduleconfig", "CONFIG", DNNrocketUtils.GetEditCulture(), -1,moduleid);
                postInfo.ParentItemId = info.ItemID;
                objCtrl.SaveData(moduleid.ToString(), rocketInterface.EntityTypeCode, postInfo,-1, moduleid);
                return GetEditData(moduleid,rocketInterface, postInfo);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static String ResetRocketMod(int moduleid, DNNrocketInterface rocketInterface)
        {
            try
            {
                ConfigUtils.DeleteConfig(moduleid);
                return GetDashBoard(moduleid, rocketInterface);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static String GetDashBoard(int moduleid, DNNrocketInterface rocketInterface)
        {
            try
            {
                var controlRelPath = rocketInterface.TemplateRelPath;
                if (controlRelPath == "") controlRelPath = ControlRelPath;

                var themeFolder = rocketInterface.DefaultTheme;
                var razortemplate = "dashboard.cshtml";

                var passSettings = rocketInterface.ToDictionary();
                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());
                var objCtrl = new DNNrocketController();

                passSettings.Add("mappathAppThemeFolder", _appthemeMapPath);
                
                var info = objCtrl.GetData("moduleconfig", "CONFIG", DNNrocketUtils.GetEditCulture(), -1, moduleid);
                return DNNrocketUtils.RazorDetail(razorTempl, info, passSettings);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static String GetData(int moduleid, DNNrocketInterface rocketInterface, SimplisityInfo postInfo)
        {

            try
            {
                var strOut = "";
                if (ConfigUtils.HasConfig(moduleid))
                {
                    var objCtrl = new DNNrocketController();

                    var razortemplate = "view.cshtml";
                    var configInfo = objCtrl.GetData("moduleconfig", "CONFIG", DNNrocketUtils.GetEditCulture(), -1, moduleid);
                    var themeFolder = configInfo.GetXmlProperty("genxml/select/apptheme");
                    postInfo.ModuleId = moduleid; // make sure we have correct moduleid.
                    strOut = GetList(rocketInterface, postInfo, _appthemeRelPath, razortemplate, themeFolder);
                    if (strOut == "") strOut = "ERROR: No data returned for " + _appthemeMapPath + "\\" + themeFolder + "\\default\\" + razortemplate;
                }
                else
                {
                    strOut = ConfigUtils.GetSetup(moduleid, rocketInterface);
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
