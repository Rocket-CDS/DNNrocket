using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;

namespace DNNrocket.Images
{
    public class StartConnect : DNNrocketAPI.APInterface
    {
        private string _appthemeRelPath;
        private string _appthemeMapPath;
        private SimplisityInfo _postInfo;
        private SimplisityInfo _paramInfo;
        private CommandSecurity _commandSecurity;
        private DNNrocketInterface _rocketInterface;

        public override Dictionary<string, string> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            var strOut = "ERROR"; // return ERROR if not matching commands.

            paramCmd = paramCmd.ToLower();

            _rocketInterface = new DNNrocketInterface(interfaceInfo);

            var appPath = _rocketInterface.TemplateRelPath;
            if (appPath == "") appPath = "/DesktopModules/DNNrocket/Images";
            _appthemeRelPath = appPath;
            _appthemeMapPath = DNNrocketUtils.MapPath(_appthemeRelPath);
            _postInfo = postInfo;
            _paramInfo = paramInfo;

            _commandSecurity = new CommandSecurity(-1, -1, _rocketInterface);
            _commandSecurity.AddCommand("rocketimages_upload", true);
            _commandSecurity.AddCommand("rocketimages_delete", true);
            _commandSecurity.AddCommand("rocketimages_list", false);

            if (!_commandSecurity.HasSecurityAccess(paramCmd))
            {
                strOut = UserUtils.LoginForm(systemInfo, postInfo, _rocketInterface.InterfaceKey, DNNrocketUtils.GetCurrentUserId());
                return ReturnString(strOut);
            }

            try
            {
                switch (paramCmd)
                {
                    case "rocketimages_upload":
                        strOut = UploadImageToFolder();
                        if (strOut == "") strOut = ListData();
                        break;
                    case "rocketimages_delete":
                        DeleteImages();
                        strOut = ListData();
                        break;
                    case "rocketimages_list":
                        strOut = ListData();
                        break;
                }
            }
            catch (Exception ex)
            {
                strOut = ex.ToString();
            }

            return ReturnString(strOut);
        }

        public Dictionary<string, string> ReturnString(string strOut, string jsonOut = "")
        {
            var rtnDic = new Dictionary<string, string>();
            rtnDic.Add("outputhtml", strOut);
            rtnDic.Add("outputjson", jsonOut);
            return rtnDic;
        }


        public String ListData()
        {
            try
            {
                return DNNrocketUtils.RenderImageSelect(100, _paramInfo.GetXmlPropertyBool("genxml/hidden/singleselect"), _paramInfo.GetXmlPropertyBool("genxml/hidden/autoreturn"), _paramInfo.GetXmlProperty("genxml/hidden/imagefolder"));
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string UploadImageToFolder()
        {
            var userid = DNNrocketUtils.GetCurrentUserId(); // prefix to filename on upload.
            var imagefolder = _paramInfo.GetXmlProperty("genxml/hidden/imagefolder");
            if (imagefolder == "") imagefolder = "images";
            var uploadFolderPath = imagefolder;
            if (!uploadFolderPath.Contains("/")) uploadFolderPath = DNNrocketUtils.HomeDNNrocketDirectoryRel() + "/" + imagefolder;
            var imageDirectory = DNNrocketUtils.MapPath(uploadFolderPath);
            if (!Directory.Exists(imageDirectory)) Directory.CreateDirectory(imageDirectory);
            var strOut = "";
            var createseo = _paramInfo.GetXmlPropertyBool("genxml/hidden/createseo");
            var resize = _paramInfo.GetXmlPropertyInt("genxml/hidden/imageresize");
            if (resize == 0) resize = 640;
            var fileuploadlist = _paramInfo.GetXmlProperty("genxml/hidden/fileuploadlist");
            if (fileuploadlist != "")
            {
                foreach (var f in fileuploadlist.Split(';'))
                {
                    if (f != "")
                    {
                        var friendlyname = GeneralUtils.DeCode(f);
                        var userfilename = userid + "_" + friendlyname;
                        var unqName = DNNrocketUtils.GetUniqueFileName(friendlyname, imageDirectory);
                        var fname = ImgUtils.ResizeImage(DNNrocketUtils.TempDirectoryMapPath() + "\\" + userfilename, imageDirectory + "\\" + unqName, resize);
                        if (!File.Exists(fname)) return "ERROR: " + fname;
                        if (createseo)
                        {
                            var imageDirectorySEO = imageDirectory + "\\seo";
                            if (!Directory.Exists(imageDirectorySEO)) Directory.CreateDirectory(imageDirectorySEO);
                            ImgUtils.CopyImageForSEO(DNNrocketUtils.TempDirectoryMapPath() + "\\" + userfilename, imageDirectorySEO, unqName);
                        }

                        File.Delete(DNNrocketUtils.TempDirectoryMapPath() + "\\" + userfilename);
                    }
                }

            }

            return strOut;
        }

        public void DeleteImages()
        {
            var uploadrelfolder = _paramInfo.GetXmlProperty("genxml/hidden/uploadrelfolder");
            if (uploadrelfolder != "")
            {
                var imageDirectory = DNNrocketUtils.MapPath(uploadrelfolder);
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

                CacheUtils.ClearAllCache();
                DNNrocketUtils.ClearPortalCache();

            }

        }



    }
}
