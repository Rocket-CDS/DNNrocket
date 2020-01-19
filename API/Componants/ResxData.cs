using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DNNrocketAPI.Componants
{
    public class ResxData
    {
        public ResxData(string resxFileMapPath)
        {
            Exists = false;
            ResxXmlData = new XmlDocument();
            DataDictionary = new Dictionary<string, string>();
            FileMapPath = resxFileMapPath;
            FileName = Path.GetFileName(FileMapPath);
            CultureCode = "";
            var templateNameWithoutExtension = Path.GetFileNameWithoutExtension(FileMapPath);
            var t = templateNameWithoutExtension.Split('.');
            if (t.Length == 2) CultureCode = t[1];

            if (File.Exists(resxFileMapPath))
            {
                ResxFileData = FileUtils.ReadFile(FileMapPath);
                ResxXmlData.Load(FileMapPath);
                BuildDictionary();
                Exists = true;
            }
        }

        private void BuildDictionary()
        {
            var nodList = ResxXmlData.SelectNodes("root/data");
            foreach (XmlNode n in nodList)
            {
                var key = n.SelectSingleNode("@name").InnerText;
                if (DataDictionary.ContainsKey(key)) DataDictionary.Remove(key);
                DataDictionary.Add(key, n.SelectSingleNode("value").InnerText);
            }
        }

        public void AddField(string key, string value)
        {
            if (!DataDictionary.ContainsKey(key))
            {
                DataDictionary.Add(key, value);
                RemoveField(key); // could be out of line with dictionary
                AddXmlField(key, value);
            }
            else
            {
                DataDictionary.Remove(key);
                RemoveField(key);
                DataDictionary.Add(key, value);
                AddXmlField(key, value);
            }
        }
        private void AddXmlField(string key, string value)
        {
            XmlElement data = ResxXmlData.CreateElement("data");
            data.SetAttribute("name", key);
            data.SetAttribute("xml:space", "preserve");
            XmlElement valueNode = ResxXmlData.CreateElement("value");
            valueNode.InnerText = WebUtility.HtmlEncode(value);
            data.AppendChild(valueNode);
            ResxXmlData.DocumentElement.AppendChild(data);
        }
        public void RemoveField(string key)
        {
            if (DataDictionary.ContainsKey(key))
            {
                DataDictionary.Remove(key);                
                XmlNode childNode = ResxXmlData.SelectSingleNode("/root/data[@name='" + key + "']"); 
                if (childNode != null) childNode.ParentNode.RemoveChild(childNode);
            }
        }
        public void RemoveAllFields()
        {
            foreach (var r in DataDictionary)
            {
                XmlNode childNode = ResxXmlData.SelectSingleNode("/root/data[@name='" + r.Key + "']");
                if (childNode != null) childNode.ParentNode.RemoveChild(childNode);
            }
            DataDictionary = new Dictionary<string, string>();
            Save();
            ResxXmlData.Load(FileMapPath);
            BuildDictionary();
        }


        public void Save()
        {
            ResxXmlData.Save(FileMapPath);
            ResxXmlData.Load(FileMapPath);
            BuildDictionary();
        }

        public string GetJsonResx()
        {
            var jsonStr = "{\"resx\":[";
            var lp = 1;
            foreach (var j in DataDictionary)
            {
                jsonStr += "{\"id\":\"name_" + lp + "\",\"value\":\"" + j.Key.Replace("\"", "") + "\",\"row\":\"" + lp + "\"},";
                jsonStr += "{\"id\":\"value_" + lp + "\",\"value\":\"" + j.Value.Replace("\"", "") + "\",\"row\":\"" + lp + "\"},";
                lp += 1;
            }
            jsonStr = jsonStr.TrimEnd(',') + "]}";
            return jsonStr;
        }


        public void Delete()
        {
            if (File.Exists(FileMapPath)) File.Delete(FileMapPath);
        }

        public XmlDocument ResxXmlData { get; set; }
        public string ResxFileData { get; set; }
        public string FileName { get; set; }
        public string CultureCode { get; set; }
        public string FileMapPath { get; set; }
        public bool Exists { get; set; }
        public Dictionary<string,string> DataDictionary { get; set; }


    }
}
