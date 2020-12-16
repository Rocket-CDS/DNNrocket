using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DNNrocketAPI.Components
{
    public class ResxData
    {
        private string _englishFileNamePath; 
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

            _englishFileNamePath = Path.GetFullPath(resxFileMapPath).Replace(Path.GetFileName(FileMapPath), "") + t[0] + Path.GetExtension(FileMapPath);

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
            var DataDictionary1 = new Dictionary<string, string>();
            // load english as default keys (add then to other languages if not in langauge file)
            var englishKeys = new Dictionary<string, string>();
            if (File.Exists(_englishFileNamePath))
            {
                var englishXmlData = new XmlDocument();
                englishXmlData.Load(_englishFileNamePath);
                var engNodList = englishXmlData.SelectNodes("root/data");
                foreach (XmlNode n in engNodList)
                {
                    var key = n.SelectSingleNode("@name").InnerText;
                    if (DataDictionary1.ContainsKey(key)) DataDictionary1.Remove(key);
                    englishKeys.Add(key, n.SelectSingleNode("value").InnerText);
                }                
            }

            var nodList = ResxXmlData.SelectNodes("root/data");
            foreach (XmlNode n in nodList)
            {
                var key = n.SelectSingleNode("@name").InnerText;
                if (DataDictionary1.ContainsKey(key)) DataDictionary1.Remove(key);
                DataDictionary1.Add(key, n.SelectSingleNode("value").InnerText);
            }

            // add any missing English keys
            foreach (var d in englishKeys)
            {
                if (!DataDictionary1.ContainsKey(d.Key))
                {
                    DataDictionary1.Add(d.Key,d.Value + " **");
                }
            }

            var l = DataDictionary1.OrderBy(key => key.Key);
            DataDictionary = l.ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);

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
            Update();
            ResxXmlData.Load(FileMapPath);
            BuildDictionary();
        }
        public void Rebuild(AppThemeLimpet appTheme)
        {
            var fieldData = appTheme.GetFieldDictionaryFields();
            foreach (var r in fieldData)
            {
                AddField(r.Key, r.Value);
            }
            Update();
            ResxXmlData.Load(FileMapPath);
            BuildDictionary();
        }
        public void Update()
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
