using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Simplisity
{
    public static class SimplisityUtils
    {        

        public static string PostData(string url, string systemkey, string simplisity_cmd, string inputjson = "", string paramjson = "", NameValueCollection reqparm = null)
        {
            if (reqparm == null) reqparm = new NameValueCollection();
            reqparm.Add("simplisity_cmd", simplisity_cmd);
            reqparm.Add("inputjson", inputjson);
            reqparm.Add("paramjson", paramjson);
            if (!url.ToLower().StartsWith("http") && !url.ToLower().StartsWith("https")) url = "http://" + url;
            return PostData(url + "?cmd=" + simplisity_cmd + "&systemkey=" + systemkey, reqparm);
        }
        public static string PostData(string url, NameValueCollection reqparm)
        {
            var responsebody = "";
            try
            {
                using (WebClient client = new WebClient())
                {
                    byte[] responsebytes = client.UploadValues(url, "POST", reqparm);
                    responsebody = Encoding.UTF8.GetString(responsebytes);
                }
            }
            catch (Exception ex)
            {
                responsebody = ex.ToString();
            }
            return responsebody;
        }

        public static string ConvertToJson(Dictionary<string, SimplisityInfo> dataObjects)
        {
            var dataInfo = new SimplisityInfo();
            foreach (var o in dataObjects)
            {
                dataInfo.SetXmlProperty("genxml/" + o.Key, "");
                var si = (SimplisityInfo)o.Value;
                si.SetXmlProperty("genxml/column/itemid", si.ItemID.ToString());
                si.SetXmlProperty("genxml/column/portalid", si.PortalId.ToString());
                si.SetXmlProperty("genxml/column/moduleid", si.ModuleId.ToString());
                si.SetXmlProperty("genxml/column/typecode", si.TypeCode ?? "");
                si.SetXmlProperty("genxml/column/guidkey", si.GUIDKey ?? "");
                si.SetXmlProperty("genxml/column/xrefitemid", si.XrefItemId.ToString());
                si.SetXmlProperty("genxml/column/userid", si.UserId.ToString());
                si.SetXmlProperty("genxml/column/lang", si.Lang ?? "");
                dataInfo.AddXmlNode(o.Value.XMLData, si.RootNodeName, "genxml/" + o.Key);
            }

            dataInfo.XMLDoc.DocumentElement.SetAttribute("xmlns:json", "http://james.newtonking.com/projects/json");

            //Create a new attribute
            XmlAttribute attr = dataInfo.XMLDoc.CreateAttribute("json", "Array", "http://james.newtonking.com/projects/json");
            attr.Value = "true";

            //Add the attribute to the node     
            var nodList = dataInfo.XMLDoc.SelectNodes("genxml/data/genxml/*[@list='true']/genxml");
            foreach (XmlNode n in nodList)
            {
                n.Attributes.SetNamedItem(attr);
            }
            var nodList2 = dataInfo.XMLDoc.SelectNodes("genxml/data/genxml/lang/genxml/*[@list='true']/genxml");
            foreach (XmlNode n in nodList2)
            {
                n.Attributes.SetNamedItem(attr);
            }

            var doc = XElement.Parse(dataInfo.XMLData);
            var cdata = doc.DescendantNodes().OfType<XCData>().ToList();
            foreach (var cd in cdata)
            {
                cd.Parent.Add(cd.Value);
                cd.Remove();
            }
            return Newtonsoft.Json.JsonConvert.SerializeXNode(doc, Newtonsoft.Json.Formatting.Indented);
        }

    }
}
