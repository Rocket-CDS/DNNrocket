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
            IsValid = true;
            if (String.IsNullOrEmpty(_systemGlobalData.FtpServer) || String.IsNullOrEmpty(_systemGlobalData.FtpUserName) || String.IsNullOrEmpty(_systemGlobalData.FtpPassword)) IsValid = false;
        }

        public bool IsValid { get; private set; }

        public string UploadAppTheme(AppTheme appTheme)
        {
            if (!IsValid) return "";

            var exportZipMapPath = appTheme.ExportZipFile();
            var filename = Path.GetFileName(exportZipMapPath);

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
                    var xmlMapPath = DNNrocketUtils.TempDirectoryMapPath() + "\\" + appTheme.AppThemeFolder + ".xml";
                    var sInfo = new SimplisityRecord();
                    sInfo.SetXmlProperty("genxml/hidden/appthemefolder", appTheme.AppThemeFolder);
                    sInfo.SetXmlProperty("genxml/hidden/summary", appTheme.AppSummary);
                    sInfo.SetXmlProperty("genxml/hidden/latestversion", appTheme.LatestVersionFolder);
                    sInfo.SetXmlProperty("genxml/hidden/latestrev", appTheme.LatestRev.ToString());
                    var logoMapPath = DNNrocketUtils.MapPath(appTheme.Logo);
                    sInfo.SetXmlProperty("genxml/hidden/logo", Path.GetFileName(logoMapPath));
                    if (File.Exists(logoMapPath))
                    {
                        var newImage = ImgUtils.CreateThumbnail(logoMapPath, Convert.ToInt32(140), Convert.ToInt32(140));

                        // Convert the image to byte[]
                        System.IO.MemoryStream stream = new System.IO.MemoryStream();
                        newImage.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                        byte[] imageBytes = stream.ToArray();
                        string base64String = Convert.ToBase64String(imageBytes);
                        sInfo.SetXmlProperty("genxml/hidden/logobase64", base64String);   
                    }

                    var updateXml = sInfo.ToXmlItem();
                    FileUtils.SaveFile(xmlMapPath, updateXml);
                    client.UploadFile(urixml + "/" + Path.GetFileName(xmlMapPath), WebRequestMethods.Ftp.UploadFile, xmlMapPath);
                    UploadChangedXmlIndex(appTheme.AppThemeFolder, updateXml);
                }
            }
            catch (Exception ex)
            {
                DNNrocketUtils.LogException(ex);
                return "[FAIL]  uri:" + _baseuri + " exportZipMapPath:" + exportZipMapPath + " Error:" + ex.ToString();
            }
            return "OK";
        }

        private void UploadChangedXmlIndex(string appThemeFolder, string updateXml = "")
        {
            if (IsValid)
            {
                var uriidx = _baseuri + "/idx";
                var reindexuri = uriidx + "/list.xml";
                var xmlIdx = Download(reindexuri);

                if (xmlIdx == "" || xmlIdx == "FAIL")
                {
                    ReindexPrivateXmlList();
                }
                else
                {
                    if (updateXml != "")
                    {
                        var idxInfo = new SimplisityRecord();
                        idxInfo.XMLData = xmlIdx;
                        idxInfo.RemoveRecordListItem("idx", "genxml/hidden/appthemefolder", appThemeFolder);
                        var updateInfo = new SimplisityRecord();
                        updateInfo.FromXmlItem(updateXml);
                        idxInfo.AddListItem("idx", updateInfo.XMLData);
                        UploadXmlIndex(idxInfo.XMLData);
                    }
                }
            }
        }

        private void UploadXmlIndex(string fulllistxml)
        {
            if (IsValid)
            {
                var uriidx = _baseuri + "/idx";
                CreateFTPDirectory(uriidx);

                using (var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(_systemGlobalData.FtpUserName, _systemGlobalData.FtpPassword);
                    var idxMapPath = DNNrocketUtils.TempDirectoryMapPath() + "\\list.xml";

                    FileUtils.SaveFile(idxMapPath, fulllistxml);
                    client.UploadFile(uriidx + "/list.xml", WebRequestMethods.Ftp.UploadFile, idxMapPath);
                }
            }
        }
        public string ReindexPrivateXmlList()
        {
            if (IsValid)
            {
                var ftpidx = "<genxml>";
                ftpidx += "<idx list=\"true\">";

                var l = DownloadAppThemeXmlList();
                foreach (var x in l)
                {
                    ftpidx += x.XMLData;
                }
                ftpidx += "</idx>";
                ftpidx += "</genxml>";
                UploadXmlIndex(ftpidx);
                return ftpidx;
            }
            return "<genxml></genxml>";
        }


        public bool CreateFTPDirectory(string directory)
        {
            if (!IsValid) return false;
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
                //DNNrocketUtils.LogException(ex);
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
        public string DownloadAppThemeToFile(string appThemeFolder, string destinationMapPath)
        {
            if (!IsValid) return "FAIL";
            try
            {
                var uri = _baseuri + "/zip/" + appThemeFolder + ".zip";
                DownloadZip(uri, destinationMapPath);
            }
            catch (Exception exc)
            {
                DNNrocketUtils.LogException(exc);
                return "FAIL";
            }
            return "OK";
        }

        public void DownloadZip(string uri, string destinationmapPath)
        {
            if (IsValid)
            {
                WebClient client = new WebClient();
                client.Credentials = new NetworkCredential(_systemGlobalData.FtpUserName, _systemGlobalData.FtpPassword);
                client.DownloadFile(uri, destinationmapPath);
            }
        }

        public string Download(string uri)
        {
            if (!IsValid) return "FAIL";
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
                request.KeepAlive = false;
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

        public List<SimplisityRecord> DownloadAppThemeXmlIndexList()
        {
            var rtnList = new List<SimplisityRecord>();
            if (!IsValid) return rtnList;

            var uriidx = _baseuri + "/idx";
            var reindexuri = uriidx + "/list.xml";
            var xmlIdx = Download(reindexuri);

            if (xmlIdx == "" || xmlIdx == "FAIL")
            {
                xmlIdx = ReindexPrivateXmlList();
            }

            var sRec = new SimplisityRecord();
            sRec.XMLData = xmlIdx;

            var xmlReclist = sRec.GetRecordList("idx");
            foreach (var n in xmlReclist)
            {
                rtnList.Add(n);
            }
            return rtnList;
        }

        public List<SimplisityRecord> DownloadAppThemeXmlList()
        {
            var rtnList = new List<SimplisityRecord>();
            if (!IsValid) return rtnList;
            var namelist = ListXmlFiles();
            foreach (var n in namelist)
            {
                var uri = _baseuri + "/xml/" + n;
                var xmlDownload = Download(uri);
                if (xmlDownload != "FAIL")
                {
                    var sInfo = new SimplisityRecord();
                    sInfo.FromXmlItem(xmlDownload);
                    rtnList.Add(sInfo);
                }
            }
            return rtnList;
        }
        private List<string> ListXmlFiles()
        {
            try
            {
                if (!IsValid) 
                {
                    var uri = _baseuri + "/xml";
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
                    request.KeepAlive = false;
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
                return new List<string>();
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }


    }
}
