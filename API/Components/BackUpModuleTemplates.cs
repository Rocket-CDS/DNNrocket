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
    public class BackUpModuleTemplates
    {
        // Backup local Templates froim file system

        public BackUpModuleTemplates(int portalId, string fileMapPath,int moduleId, string systemKey)
        {
            PortalId = portalId;
            SystemKey = systemKey;
            ModuleId = moduleId;
            FileMapPath = fileMapPath;
            ItemList = new List<SimplisityInfo>();
            Info = new SimplisityInfo();

            Load();
        }
        private void Load()
        {
            BackUpDate = DateTime.Now;
            string backUpXml;
            if (File.Exists(FileMapPath))
            {
                var s = Path.GetFileNameWithoutExtension(FileMapPath).Split('_');
                if (s.Length == 2 && GeneralUtils.IsNumeric(s[0])) BackUpDate = DateTime.FromFileTime(Convert.ToInt64(s[0]));
                backUpXml = FileUtils.ReadFile(FileMapPath);
            }
            else
            {
                var appThemeMod = new AppThemeModule(PortalId, ModuleId, SystemKey);
                backUpXml = appThemeMod.ExportModuleLevelTemplateXml();
            }
            if (!String.IsNullOrEmpty(backUpXml)) LoadTemplateList(backUpXml);
        }

        private void LoadTemplateList(string backUpXml)
        {
            ItemList = new List<SimplisityInfo>();
            Info = new SimplisityInfo();
            Info.XMLData = backUpXml;
            var nodList = Info.XMLDoc.SelectNodes("templates/*");
            foreach (XmlNode nod in nodList)
            {
                var importXml = nod.OuterXml;
                var importInfo = new SimplisityInfo();
                importInfo.FromXmlItem(importXml);
                ItemList.Add(importInfo);
            }
        }

        public void BackUp()
        {
            Save();
            Load();
        }
        public void RestoreData()
        {
            // do a second loop for index, so we have all correct data updated
            foreach (var s in ItemList)
            {
                var text = GeneralUtils.DeCode(s.GetXmlProperty("template/text"));
                FileUtils.SaveFile(s.GetXmlProperty("template/mappath"), text);
            }
        }

        public void Save()
        {
            var backUpData = "<templates>";
            foreach (var i in ItemList)
            {
                backUpData += i.ToXmlItem();
            }
            backUpData += "</templates>";
            FileUtils.SaveFile(FileMapPath, backUpData);
        }

        public List<SimplisityInfo> ItemList { get; set; }
        public string FileMapPath { get; set; }
        public SimplisityInfo Info { get; set; }
        public DateTime BackUpDate { get; set; }
        public string SystemKey { get; set; }
        public int ModuleId { get; set; }
        public int PortalId { get; set; }

    }

}
