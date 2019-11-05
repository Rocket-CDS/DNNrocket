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
        private string _ftpUsername;
        private string _ftpPassword;
        private string _ftpServer;
        private string _ftpHomeDirectory;
        private string _publicAppThemeURI;

        public FtpConnect(string systemKey)
        {
            _systemInfoData = new SystemInfoData(systemKey);

            _ftpUsername = _systemInfoData.SystemInfo.GetXmlProperty("genxml/textbox/ftpuser");
            _ftpPassword = _systemInfoData.SystemInfo.GetXmlProperty("genxml/textbox/ftppassword");
            _ftpServer = _systemInfoData.SystemInfo.GetXmlProperty("genxml/textbox/ftpserver");
            _ftpHomeDirectory = _systemInfoData.SystemInfo.GetXmlProperty("genxml/textbox/ftphomedirectory");
            _publicAppThemeURI = _systemInfoData.SystemInfo.GetXmlProperty("genxml/textbox/publicappthemeuri");

        }

        public string UploadAppTheme(AppTheme appTheme)
        {

            var exportZipMapPath = appTheme.Export();
            var filename = Path.GetFileName(exportZipMapPath).ToLower();
            var uri = "ftp://" + _ftpServer + "/" + _ftpHomeDirectory.TrimStart('/').TrimEnd('/');
            CreateFTPDirectory(uri);
            var uriimg = uri + "/img";
            CreateFTPDirectory(uriimg);
            var urixml = uri + "/xml";
            CreateFTPDirectory(urixml);
            var urizip = uri + "/zip";
            CreateFTPDirectory(urizip);
            try
            {
                using (var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);
                    client.UploadFile(urizip + "/" + filename, WebRequestMethods.Ftp.UploadFile, exportZipMapPath);
                }
                using (var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);
                    var xmlMapPath = DNNrocketUtils.TempDirectoryMapPath() + "\\" + appTheme.AppThemeFolder.ToLower() + ".xml";
                    Simplisity.FileUtils.SaveFile(xmlMapPath, appTheme.Record.ToXmlItem());
                    client.UploadFile(urixml + "/" + Path.GetFileName(xmlMapPath), WebRequestMethods.Ftp.UploadFile, xmlMapPath);
                }
                using (var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);
                    var imgMapPath = DNNrocketUtils.MapPath(appTheme.Logo.ToLower());
                    client.UploadFile(uriimg + "/" + Path.GetFileName(imgMapPath), WebRequestMethods.Ftp.UploadFile, imgMapPath);
                }
            }
            catch (Exception ex)
            {
                return "[FAIL]  uri:" + uri + " exportZipMapPath:" + exportZipMapPath + " Error:" + ex.ToString();
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
                requestDir.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);
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

        public string DownloadPublicAppTheme(string appThemeMapPath)
        {
            try
            {
                var filename = Path.GetFileName(appThemeMapPath);
            }
            catch (Exception)
            {
                return "FAIL";
            }
            return "OK";
        }
        public string DownloadPrivateAppTheme(string appThemeMapPath)
        {
            try
            {
                var filename = Path.GetFileName(appThemeMapPath);
                // Get the object used to communicate with the server.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + _ftpHomeDirectory.TrimEnd('/') + "/" + filename);
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                // This example assumes the FTP site uses anonymous logon.
                request.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);

                Simplisity.FileUtils.SaveFile(appThemeMapPath, reader.ReadToEnd());

                reader.Close();
                response.Close();

            }
            catch (Exception)
            {
                return "FAIL";
            }
            return "OK";
        }

    }
}
