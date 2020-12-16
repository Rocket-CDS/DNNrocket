using DNNrocketAPI;
using DNNrocketAPI.Components;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;

namespace DNNrocket.Documents
{
    public class StartConnect : DNNrocketAPI.APInterface
    {
        private string _appthemeRelPath;
        private string _appthemeMapPath;
        private SimplisityInfo _postInfo;
        private SimplisityInfo _paramInfo;
        private RocketInterface _rocketInterface;

        public override Dictionary<string, object> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            var strOut = "ERROR"; // return ERROR if not matching commands.

            paramCmd = paramCmd.ToLower();

            _rocketInterface = new RocketInterface(interfaceInfo);

            var appPath = _rocketInterface.TemplateRelPath;
            if (appPath == "") appPath = "/DesktopModules/DNNrocket/Documents";
            _appthemeRelPath = appPath;
            _appthemeMapPath = DNNrocketUtils.MapPath(_appthemeRelPath);
            _postInfo = postInfo;
            _paramInfo = paramInfo;

            //[TODO : Security]
            if (false)
            {
                strOut = UserUtils.LoginForm(systemInfo, postInfo, _rocketInterface.InterfaceKey, UserUtils.GetCurrentUserId());
                return DNNrocketUtils.ReturnString(strOut);
            }

            switch (paramCmd)
            {
                case "rocketdocs_upload":
                    strOut = UploadDocumentToFolder();
                    if (strOut == "") strOut = ListData();
                    break;
                case "rocketdocs_delete":
                    DeleteDocs();
                    strOut = ListData();
                    break;
                case "rocketdocs_list":
                    strOut = ListData();
                    break;
            }

            return DNNrocketUtils.ReturnString(strOut);
        }

        public String ListData()
        {
            try
            {
                var moduleid = _paramInfo.GetXmlPropertyInt("genxml/hidden/moduleid");
                if (moduleid == 0)
                {
                    return "Invalid or Missing: ModuleId";
                }
                else
                {
                    var moduleParams = new ModuleParams(moduleid);
                    var singleselect = _paramInfo.GetXmlPropertyBool("genxml/hidden/singleselect");
                    var autoreturn = _paramInfo.GetXmlPropertyBool("genxml/hidden/autoreturn");

                    return DNNrocketUtils.RenderDocumentSelect(moduleParams, singleselect, autoreturn);
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string UploadDocumentToFolder()
        {
            var strOut = "";
            var userid = UserUtils.GetCurrentUserId(); // prefix to filename on upload.
            var moduleId = _paramInfo.GetXmlPropertyInt("genxml/hidden/moduleid");
            if (moduleId > 0)
            {
                var modParams = new ModuleParams(moduleId);
                if (!Directory.Exists(modParams.DocumentFolderMapPath)) Directory.CreateDirectory(modParams.DocumentFolderMapPath);
                var fileuploadlist = _paramInfo.GetXmlProperty("genxml/hidden/fileuploadlist");
                if (fileuploadlist != "")
                {
                    foreach (var f in fileuploadlist.Split(';'))
                    {
                        if (f != "")
                        {
                            var friendlyname = GeneralUtils.DeCode(f);
                            var userfilename = userid + "_" + friendlyname;
                            File.Copy(PortalUtils.TempDirectoryMapPath() + "\\" + userfilename, modParams.DocumentFolderMapPath + "\\" + friendlyname, true);
                            File.Delete(PortalUtils.TempDirectoryMapPath() + "\\" + userfilename);
                        }
                    }

                }
            }

            return strOut;
        }

        public void DeleteDocs()
        {
            var docfolder = _postInfo.GetXmlProperty("genxml/hidden/documentfolder");
            if (docfolder == "") docfolder = "docs";
            var docDirectory = PortalUtils.HomeDNNrocketDirectoryMapPath() + "\\" + docfolder;
            var docList = _postInfo.GetXmlProperty("genxml/hidden/dnnrocket-documentlist").Split(';');
            foreach (var i in docList)
            {
                if (i != "")
                {
                    var documentname = GeneralUtils.DeCode(i);
                    var docFile = docDirectory + "\\" + documentname;
                    if (File.Exists(docFile))
                    {
                        File.Delete(docFile);
                    }
                }
            }

        }



    }
}
