using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DNNrocketAPI.Components
{
    public class HttpConnect
    {
        private SystemLimpet _systemData;
        private SystemGlobalData _systemGlobalData;
        private string _baseuri;
        public HttpConnect(string selectedSystemKey)
        {
            var imgidxFolder = DNNrocketUtils.SystemThemeImgDirectoryMapPath();
            if (!Directory.Exists(imgidxFolder)) Directory.CreateDirectory(imgidxFolder);

            _systemData = new SystemLimpet(selectedSystemKey);
            _systemGlobalData = new SystemGlobalData();
           // _baseuri = _systemGlobalData.PublicAppThemeURI.TrimStart('/').TrimEnd('/') + "/" + _systemData.SystemKey;
        }
        public string DownloadAppThemeToFile(string appThemeFolder, string destinationMapPath)
        {
            try
            {
                var uri = _baseuri + "/zip/" + appThemeFolder + ".zip";
                DownloadZip(uri, destinationMapPath);
            }
            catch (Exception exc)
            {
                LogUtils.LogException(exc);
                return "FAIL";
            }
            return "OK";
        }

        public void DownloadZip(string uri, string destinationmapPath)
        {
            WebClient client = new WebClient();
            client.DownloadFile(uri, destinationmapPath);
        }

        public string DownloadAppThemeXml(string appThemeFolder)
        {
            try
            {
                var uri = _baseuri + "/xml/" + appThemeFolder + ".xml";
                return Download(uri);
            }
            catch (Exception exc)
            {
                LogUtils.LogException(exc);
            }
            return "<genxml></genxml>";
        }

        public void DownloadAppThemeXmlToFile(string appThemeFolder, string destinatioFileMapPath)
        {
            try
            {
                WebClient client = new WebClient();
                var uri = _baseuri + "/xml/" + appThemeFolder + ".xml";
                client.DownloadFile(uri, destinatioFileMapPath);
            }
            catch (Exception exc)
            {
                LogUtils.LogException(exc);
            }
        }


        public string Download(string uri)
        {
            try
            {
                WebClient client = new WebClient();
                var rtnString = client.DownloadString(uri);
                return rtnString;
            }
            catch (Exception)
            {
                return "FAIL";
            }
        }
        public void DownloadImageToFile(string uri, string destinationMapPath)
        {
            try
            {
                WebClient client = new WebClient();
                client.DownloadFile(uri, destinationMapPath);
            }
            catch (Exception exc)
            {
                LogUtils.LogException(exc);
            }
        }
        public List<SimplisityRecord> DownloadAppThemeXmlList()
        {
            try
            {
                var uri = _baseuri + "/idx/list.xml";
                var xmlIndexList = Download(uri);
                var sRec = new SimplisityRecord();
                sRec.XMLData = xmlIndexList;
                var idxList =  sRec.GetRecordList("idx");

                // download index image, to display on list.
                foreach (var sRec2 in idxList)
                {
                    var imgLogo = sRec2.GetXmlProperty("genxml/hidden/logo");
                    var localMapPath = DNNrocketUtils.SystemThemeImgDirectoryMapPath() + "\\" + imgLogo;
                    if (!File.Exists(localMapPath))
                    {
                        uri = _baseuri + "/xml/" + imgLogo;
                        DownloadImageToFile(uri, localMapPath);
                    }
                }
                return idxList;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

    }
}
