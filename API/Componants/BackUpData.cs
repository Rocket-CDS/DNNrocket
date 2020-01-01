using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DNNrocketAPI.Componants
{
    public class BackUpData
    {

        public BackUpData(string fileMapPath)
        {
            Exists = false;
            FileMapPath = fileMapPath;
            if (File.Exists(FileMapPath))
            {
                Exists = true;

                BackUpDate = DateTime.Now;
                var s = Path.GetFileNameWithoutExtension(fileMapPath).Split('_');
                if (s.Length == 2 && GeneralUtils.IsNumeric(s[0])) BackUpDate = DateTime.FromFileTime(Convert.ToInt64(s[0]));
                var BackUpXml = FileUtils.ReadFile(fileMapPath);
                Info = new SimplisityInfo(BackUpXml);
                var nodList = Info.XMLDoc.SelectNodes("root/*");
                foreach (XmlNode nod in nodList)
                {
                    var importXml = nod.OuterXml;
                    var importInfo = new SimplisityInfo();
                    importInfo.FromXmlItem(importXml);
                    ItemList.Add(importInfo);
                }
            }
        }

        public void RestoreData(string tableName = "DNNrocket")
        {
            var objCtrl = new DNNrocketController();
            foreach (var s in ItemList)
            {
                objCtrl.Update(s, tableName);
            }
        }

        public List<SimplisityInfo> ItemList { get; set; }
        public string FileMapPath { get; set; }
        public bool Exists { get; set; }
        public SimplisityInfo Info { get; set; }
        public DateTime BackUpDate { get; set; }


    }
}
