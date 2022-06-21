using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DNNrocketAPI.Components
{
    public class BackUpData
    {
        // not same as export.  
        // A backup and restore will use the same record and itemid.
        // Import and Export are designed to work across portals and will therefore delete and create records.  

        public BackUpData(string fileMapPath)
        {
            Exists = false;
            FileMapPath = fileMapPath;
            Load();
        }
        private void Load()
        {
            Info = new SimplisityInfo();
            ItemList = new List<SimplisityInfo>();
            BackUpDate = DateTime.Now;
            if (File.Exists(FileMapPath))
            {
                Exists = true;
                var s = Path.GetFileNameWithoutExtension(FileMapPath).Split('_');
                if (s.Length == 2 && GeneralUtils.IsNumeric(s[0])) BackUpDate = DateTime.FromFileTime(Convert.ToInt64(s[0]));

                var d = Path.GetDirectoryName(FileMapPath);
                SystemKey = new DirectoryInfo(d).Name;

                var BackUpXml = FileUtils.ReadFile(FileMapPath);
                Info = new SimplisityInfo();
                Info.XMLData = BackUpXml;
                var nodList = Info.XMLDoc.SelectNodes("backup/item");
                foreach (XmlNode nod in nodList)
                {
                    var importXml = nod.OuterXml;
                    var importInfo = new SimplisityInfo();
                    importInfo.FromXmlItem(importXml);
                    ItemList.Add(importInfo);
                }
            }
        }
        public void BackUp(List<SimplisityInfo> list)
        {
            ItemList = new List<SimplisityInfo>();
            foreach (var i in list)
            {
                ItemList.Add(i);
            }
            Save();
            Load();
        }
        public void RestoreData(int portalId, string systemKey)
        {
            var objCtrl = new DNNrocketController();
            foreach (var s in ItemList)
            {
                objCtrl.Update(s, DatabaseTable);
            }
            // do a second loop for index, so we have all correct data updated
            foreach (var s in ItemList)
            {
                if (!s.TypeCode.EndsWith("LANG") && s.Lang == "")
                {
                    // recreate the IDX record.
                    var idxInfo = objCtrl.GetInfo(s.ItemID, s.Lang, DatabaseTable);
                    objCtrl.RebuildIndex(portalId, idxInfo.ItemID, systemKey, DatabaseTable);
                    objCtrl.RebuildLangIndex(idxInfo.PortalId, idxInfo.ItemID, DatabaseTable);
                }
            }
        }

        public void Save()
        {
            var backUpData = "<backup>";
            backUpData += "<backuptext><![CDATA[" + BackUpText + "]]></backuptext>";
            foreach (var i in ItemList)
            {
                backUpData += i.ToXmlItem();
            }
            backUpData += "</backup>";
            FileUtils.SaveFile(FileMapPath, backUpData);
        }

        public List<SimplisityInfo> ItemList { get; set; }
        public string FileMapPath { get; set; }
        public bool Exists { get; set; }
        public SimplisityInfo Info { get; set; }
        public DateTime BackUpDate { get; set; }
        public string SystemKey { get; set; }
        public string BackUpText { get; set; }
        public string DatabaseTable
        {
            get
            {
                var s = new SystemLimpet(SystemKey);
                if (s == null || !s.Exists) return "";
                return s.DatabaseTable;
            }
        }


    }

}
