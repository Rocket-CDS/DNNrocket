using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;

namespace DNNrocket.Images
{
    public class startconnect : DNNrocketAPI.APInterface
    {
        private static string _appthemeRelPath;
        private static string _appthemeMapPath;
        private static SimplisityInfo _postInfo;
        private static CommandSecurity _commandSecurity;
        private static DNNrocketInterface _rocketInterface;

        public override Dictionary<string, string> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, string userHostAddress, string langRequired = "")
        {
            var strOut = "ERROR"; // return ERROR if not matching commands.

            paramCmd = paramCmd.ToLower();

            _rocketInterface = new DNNrocketInterface(interfaceInfo);

            var appPath = _rocketInterface.TemplateRelPath;
            if (appPath == "") appPath = "/DesktopModules/DNNrocket/Images";
            _appthemeRelPath = appPath;
            _appthemeMapPath = DNNrocketUtils.MapPath(_appthemeRelPath);
            _postInfo = postInfo;

            _commandSecurity = new CommandSecurity(-1, -1, _rocketInterface);
            _commandSecurity.AddCommand("rocketimages_upload", true);
            _commandSecurity.AddCommand("rocketimages_delete", true);
            _commandSecurity.AddCommand("rocketimages_list", false);

            if (!_commandSecurity.HasSecurityAccess(paramCmd))
            {
                strOut = LoginUtils.LoginForm(systemInfo, postInfo, _rocketInterface.InterfaceKey, DNNrocketUtils.GetCurrentUserId());
                return ReturnString(strOut);
            }

            switch (paramCmd)
            {
                case "rocketimages_upload":
                    UploadImageToFolder();
                    strOut = ListData();
                    break;
                case "rocketimages_delete":
                    DeleteImages();
                    strOut = ListData();
                    break;
                case "rocketimages_list":
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
                return DNNrocketUtils.RenderImageSelect(new SimplisityRazor(), 100, _postInfo.GetXmlPropertyBool("genxml/hidden/singleselect"), _postInfo.GetXmlPropertyBool("genxml/hidden/autoreturn"), _postInfo.GetXmlProperty("genxml/hidden/imagefolder"));
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static string UploadImageToFolder()
        {
            var userid = DNNrocketUtils.GetCurrentUserId(); // prefix to filename on upload.

            var imagefolder = _postInfo.GetXmlProperty("genxml/hidden/imagefolder");
            if (imagefolder == "") imagefolder = "images";

            var imageDirectory = DNNrocketUtils.HomeDNNrocketDirectory() + "\\" + imagefolder;
            if (!Directory.Exists(imageDirectory)) Directory.CreateDirectory(imageDirectory);

            var strOut = "";
            var createseo = _postInfo.GetXmlPropertyBool("genxml/hidden/createseo");
            var resize = _postInfo.GetXmlPropertyInt("genxml/hidden/imageresize");
            if (resize == 0) resize = 640;
            var fileuploadlist = _postInfo.GetXmlProperty("genxml/hidden/fileuploadlist");
            if (fileuploadlist != "")
            {
                foreach (var f in fileuploadlist.Split(';'))
                {
                    if (f != "")
                    {
                        var friendlyname = GeneralUtils.DeCode(f);
                        var userfilename = userid + "_" + friendlyname;
                        var unqName = DNNrocketUtils.GetUniqueFileName(friendlyname, imageDirectory);
                        ImgUtils.ResizeImage(DNNrocketUtils.TempDirectory() + "\\" + userfilename, imageDirectory + "\\" + unqName, resize);

                        if (createseo)
                        {
                            var imageDirectorySEO = DNNrocketUtils.HomeDNNrocketDirectory() + "\\images\\seo";
                            if (!Directory.Exists(imageDirectorySEO)) Directory.CreateDirectory(imageDirectorySEO);
                            ImgUtils.CopyImageForSEO(DNNrocketUtils.TempDirectory() + "\\" + userfilename, imageDirectorySEO, unqName);
                        }

                        File.Delete(DNNrocketUtils.TempDirectory() + "\\" + userfilename);
                    }
                }

            }

            return strOut;
        }

        public static void DeleteImages()
        {
            var imagefolder = _postInfo.GetXmlProperty("genxml/hidden/imagefolder");
            if (imagefolder == "") imagefolder = "images";
            var imageDirectory = DNNrocketUtils.HomeDNNrocketDirectory() + "\\" + imagefolder;
            var imageList = _postInfo.GetXmlProperty("genxml/hidden/dnnrocket-imagelist").Split(';');
            foreach (var i in imageList)
            {
                if (i != "")
                {
                    var friendlyname = GeneralUtils.DeCode(i);
                    var imageFile = imageDirectory + "\\" + friendlyname;
                    if (File.Exists(imageFile))
                    {
                        File.Delete(imageFile);
                    }
                }
            }

        }



    }
}
