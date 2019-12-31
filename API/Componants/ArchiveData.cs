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
    public class ArchiveData
    {

        public ArchiveData(string fileMapPath)
        {
            Exists = false;
            FileMapPath = fileMapPath;
            if (File.Exists(FileMapPath))
            {
                Exists = true;
                var archiveXml = FileUtils.ReadFile(fileMapPath);
                Info = new SimplisityInfo(archiveXml);
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


    }
}
