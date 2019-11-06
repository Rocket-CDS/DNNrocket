using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Componants
{
    public class FtpConnect
    {
        private SystemInfoData _systemInfoData;
        private SystemGlobalData _systemGlobalData;
        private string _baseuri;
        public FtpConnect(string selectedSystemKey)
        {
            _systemInfoData = new SystemInfoData(selectedSystemKey);
            _systemGlobalData = new SystemGlobalData();
            _baseuri = "ftp://" + _systemGlobalData.FtpServer + "/" + _systemInfoData.FtpRoot.TrimStart('/').TrimEnd('/') + "/" + _systemInfoData.SystemKey;
        }

        public string UploadAppTheme(AppTheme appTheme)
        {

            var exportZipMapPath = appTheme.ExportZipFile();
            var filename = Path.GetFileName(exportZipMapPath).ToLower();

            var dirlist = (_systemInfoData.FtpRoot + "/" + _systemInfoData.SystemKey).Split('/');
            var createftpdir = "";
            foreach (var d in dirlist)
            {
                if (d != "")
                {
                    createftpdir += "/" + d;
                    CreateFTPDirectory("ftp://" + _systemGlobalData.FtpServer + "/" + createftpdir);
                }
            }

            var uriimg = _baseuri + "/img";
            CreateFTPDirectory(uriimg);
            var urixml = _baseuri + "/xml";
            CreateFTPDirectory(urixml);
            var urizip = _baseuri + "/zip";
            CreateFTPDirectory(urizip);
            try
            {
                using (var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(_systemGlobalData.FtpUserName, _systemGlobalData.FtpPassword);
                    client.UploadFile(urizip + "/" + filename, WebRequestMethods.Ftp.UploadFile, exportZipMapPath);
                }
                using (var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(_systemGlobalData.FtpUserName, _systemGlobalData.FtpPassword);
                    var xmlMapPath = DNNrocketUtils.TempDirectoryMapPath() + "\\" + appTheme.AppThemeFolder.ToLower() + ".xml";
                    var sInfo = new SimplisityInfo();
                    sInfo.SetXmlProperty("genxml/hidden/appthemefolder", appTheme.AppThemeFolder);
                    sInfo.SetXmlProperty("genxml/hidden/summary", appTheme.AppSummary);
                    sInfo.SetXmlProperty("genxml/hidden/latestverison", appTheme.LatestVersionFolder);
                    sInfo.SetXmlProperty("genxml/hidden/logo", Path.GetFileName(DNNrocketUtils.MapPath(appTheme.Logo)));
                    FileUtils.SaveFile(xmlMapPath, sInfo.ToXmlItem());
                    client.UploadFile(urixml + "/" + Path.GetFileName(xmlMapPath), WebRequestMethods.Ftp.UploadFile, xmlMapPath);
                }
                using (var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(_systemGlobalData.FtpUserName, _systemGlobalData.FtpPassword);
                    var imgMapPath = DNNrocketUtils.MapPath(appTheme.Logo.ToLower());
                    client.UploadFile(uriimg + "/" + Path.GetFileName(imgMapPath), WebRequestMethods.Ftp.UploadFile, imgMapPath);
                }
            }
            catch (Exception ex)
            {
                DNNrocketUtils.LogException(ex);
                return "[FAIL]  uri:" + _baseuri + " exportZipMapPath:" + exportZipMapPath + " Error:" + ex.ToString();
            }
            return "OK";
        }
        public bool CreateFTPDirectory(string directory)
        {

            try
            {
                //create the directory
                FtpWebRequest requestDir = (FtpWebRequest)FtpWebRequest.Create(new Uri(directory));
                requestDir.Method = WebRequestMethods.Ftp.MakeDirectory;
                requestDir.Credentials = new NetworkCredential(_systemGlobalData.FtpUserName, _systemGlobalData.FtpPassword);
                requestDir.UsePassive = true;
                requestDir.UseBinary = true;
                requestDir.KeepAlive = false;
                FtpWebResponse response = (FtpWebResponse)requestDir.GetResponse();
                Stream ftpStream = response.GetResponseStream();

                ftpStream.Close();
                response.Close();

                return true;
            }
            catch (WebException ex)
            {
                DNNrocketUtils.LogException(ex);
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    response.Close();
                    return true;
                }
                else
                {
                    response.Close();
                    return false;
                }
            }
        }
        public string DownloadAppThemeToFile(string destinationMapPath)
        {
            try
            {
                var filename = Path.GetFileName(destinationMapPath);
                var uri = _baseuri + "/zip/" + filename;
                FileUtils.SaveFile(destinationMapPath, Download(uri) );
            }
            catch (Exception exc)
            {
                DNNrocketUtils.LogException(exc);
                return "FAIL";
            }
            return "OK";
        }

        public string Download(string uri)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(_systemGlobalData.FtpUserName, _systemGlobalData.FtpPassword);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                var rtnString = reader.ReadToEnd();
                reader.Close();
                response.Close();
                return rtnString;
            }
            catch (Exception)
            {
                return "FAIL";
            }
        }

        public List<SimplisityInfo> DownloadAppThemeXmlList()
        {
            var rtnList = new List<SimplisityInfo>();
            try
            {
                var namelist = ListXmlFiles();
                foreach (var n in namelist)
                {
                    var uri = _baseuri + "/xml/" + n;
                    var xmlDownload = Download(uri);
                    if (xmlDownload != "FAIL")
                    {
                        var sInfo = new SimplisityInfo();
                        sInfo.XMLData = xmlDownload;
                        rtnList.Add(sInfo);
                    }
                }
            }
            catch (Exception exc)
            {
                DNNrocketUtils.LogException(exc);
            }
            return rtnList;
        }
        private List<string> ListXmlFiles()
        {
            try
            {
                var uri = _baseuri + "/xml";
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(_systemGlobalData.FtpUserName, _systemGlobalData.FtpPassword);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                string names = reader.ReadToEnd();
                reader.Close();
                response.Close();
                return names.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            catch (Exception exc)
            {
                DNNrocketUtils.LogException(exc);
            }
            return new List<string>();
        }


    }
}
