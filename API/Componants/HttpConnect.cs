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

namespace DNNrocketAPI.Componants
{
    public class HttpConnect
    {
        private SystemData _systemData;
        private SystemGlobalData _systemGlobalData;
        private string _baseuri;
        public HttpConnect(string selectedSystemKey)
        {
            _systemData = new SystemData(selectedSystemKey);
            _systemGlobalData = new SystemGlobalData();
            _baseuri = _systemGlobalData.PublicAppThemeURI.TrimStart('/').TrimEnd('/') + "/" + _systemData.SystemKey;
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
                DNNrocketUtils.LogException(exc);
                return "FAIL";
            }
            return "OK";
        }

        public void DownloadZip(string uri, string destinationmapPath)
        {
            WebClient client = new WebClient();
            client.DownloadFile(uri, destinationmapPath);
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

        public List<SimplisityRecord> DownloadAppThemeXmlList()
        {
            try
            {
                var uri = _baseuri + "/idx/list.xml";
                var xmlIndexList = Download(uri);
                var sRec = new SimplisityRecord();
                sRec.XMLData = xmlIndexList;
                return sRec.GetRecordList("idx");
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

    }
}
