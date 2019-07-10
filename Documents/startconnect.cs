using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;

namespace DNNrocket.Documents
{
    public class StartConnect : DNNrocketAPI.APInterface
    {
        private static string _appthemeRelPath;
        private static string _appthemeMapPath;
        private static SimplisityInfo _postInfo;
        private static CommandSecurity _commandSecurity;
        private static DNNrocketInterface _rocketInterface;

        public override Dictionary<string, string> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            var strOut = "ERROR"; // return ERROR if not matching commands.

            paramCmd = paramCmd.ToLower();

            _rocketInterface = new DNNrocketInterface(interfaceInfo);

            var appPath = _rocketInterface.TemplateRelPath;
            if (appPath == "") appPath = "/DesktopModules/DNNrocket/Documents";
            _appthemeRelPath = appPath;
            _appthemeMapPath = DNNrocketUtils.MapPath(_appthemeRelPath);
            _postInfo = postInfo;
            _commandSecurity = new CommandSecurity(-1, -1, _rocketInterface);
            _commandSecurity.AddCommand("rocketdocs_upload", true);
            _commandSecurity.AddCommand("rocketdocs_delete", true);
            _commandSecurity.AddCommand("rocketdocs_list", false);

            if (!_commandSecurity.HasSecurityAccess(paramCmd))
            {
                strOut = LoginUtils.LoginForm(systemInfo, postInfo, _rocketInterface.InterfaceKey, DNNrocketUtils.GetCurrentUserId());
                return ReturnString(strOut);
            }

            switch (paramCmd)
            {
                case "rocketdocs_upload":
                    UploadDocumentToFolder();
                    strOut = ListData();
                    break;
                case "rocketdocs_delete":
                    DeleteImages();
                    strOut = ListData();
                    break;
                case "rocketdocs_list":
                    strOut = ListData();
                    break;
            }

            return ReturnString(strOut);
        }

        public static Dictionary<string, string> ReturnString(string strOut, string jsonOut = "")
        {
            var rtnDic = new Dictionary<string, string>();
            rtnDic.Add("outputhtml", strOut);
            rtnDic.Add("outputjson", jsonOut);
            return rtnDic;
        }


        public static String ListData()
        {
            try
            {
                return DNNrocketUtils.RenderDocumentSelect(new SimplisityRazor(), _postInfo.GetXmlPropertyBool("genxml/hidden/singleselect"), _postInfo.GetXmlPropertyBool("genxml/hidden/autoreturn"), _postInfo.GetXmlProperty("genxml/hidden/documentfolder"));
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static string UploadDocumentToFolder()
        {
            var userid = DNNrocketUtils.GetCurrentUserId(); // prefix to filename on upload.

            var documentfolder = _postInfo.GetXmlProperty("genxml/hidden/documentfolder");
            if (documentfolder == "") documentfolder = "docs";
            var docDirectory = DNNrocketUtils.HomeDNNrocketDirectory() + "\\" + documentfolder;
            if (!Directory.Exists(docDirectory)) Directory.CreateDirectory(docDirectory);

            var strOut = "";
            var fileuploadlist = _postInfo.GetXmlProperty("genxml/hidden/fileuploadlist");
            if (fileuploadlist != "")
            {
                foreach (var f in fileuploadlist.Split(';'))
                {
                    if (f != "")
                    {
                        var friendlyname = GeneralUtils.DeCode(f);
                        var userfilename = userid + "_" + friendlyname;
                        File.Copy(DNNrocketUtils.TempDirectory() + "\\" + userfilename, docDirectory + "\\" + friendlyname,true);
                        File.Delete(DNNrocketUtils.TempDirectory() + "\\" + userfilename);
                    }
                }

            }

            return strOut;
        }

        public static void DeleteImages()
        {
            var docfolder = _postInfo.GetXmlProperty("genxml/hidden/documentfolder");
            if (docfolder == "") docfolder = "docs";
            var docDirectory = DNNrocketUtils.HomeDNNrocketDirectory() + "\\" + docfolder;
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
