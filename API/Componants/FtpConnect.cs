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

        public string UploadAppTheme(AppTheme appTheme, string ftpdomainroot = "")
        {
            try
            {
                using (var client = new WebClient())
                {
                    var appThemeMapPath = DNNrocketUtils.TempDirectoryMapPath().Trim('\\') + "\\" + appTheme.AppThemeFolder;
                    var exportZipMapPath = appTheme.Export();
                    var filename = Path.GetFileName(exportZipMapPath);
                    client.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);
                    client.UploadFile("ftp://" + ftpdomainroot.TrimEnd('/') + "/" + filename, WebRequestMethods.Ftp.UploadFile, exportZipMapPath);
                }
            }
            catch (Exception)
            {
                return "FAIL";
            }
            return "OK";
        }
        public string DownloadAppTheme(string appThemeMapPath, string ftpdomainroot = "")
        {
            try
            {
                var filename = Path.GetFileName(appThemeMapPath);
                // Get the object used to communicate with the server.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + ftpdomainroot.TrimEnd('/') + "/" + filename);
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
