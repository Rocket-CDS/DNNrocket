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
    public class RemoteModule
    {
        private DNNrocketController _objCtrl;
        private const string _tableName = "DNNrocket";
        public RemoteModule(int portalId, string moduleRef)
        {
            _objCtrl = new DNNrocketController();

            Record = _objCtrl.GetRecordByGuidKey(portalId, -1, EntityTypeCode, moduleRef, "", _tableName);
            if (Record == null)
            {
                Record = new SimplisityRecord();
                Record.PortalId = portalId;
                Record.GUIDKey = moduleRef;
                Record.TypeCode = EntityTypeCode;
            }

        }
        public int Save(SimplisityInfo paramInfo)
        {
            Record.XMLData = paramInfo.XMLData;

            return Update();
        }
        public int Update()
        {
            Record = _objCtrl.SaveRecord(Record, _tableName);
            return Record.ItemID;
        }
        public string PageUrlList(string cultureCode)
        {
            return Record.GetXmlProperty("genxml/remote/listpageurl" + cultureCode);
        }
        public string PageUrlDetail(string cultureCode)
        {
            return Record.GetXmlProperty("genxml/remote/detailpageurl" + cultureCode);
        }

        #region "properties"

        public string EntityTypeCode { get { return "RMODSETTINGS"; } }
        public SimplisityRecord Record { get; set; }
        public int ModuleId { get { return Record.ModuleId; } set { Record.ModuleId = value; } }
        public int XrefItemId { get { return Record.XrefItemId; } set { Record.XrefItemId = value; } }
        public int ParentItemId { get { return Record.ParentItemId; } set { Record.ParentItemId = value; } }
        public int ItemId { get { return Record.ItemID; } set { Record.ItemID = value; } }
        public string ModuleRef { get { return Record.GUIDKey; } set { Record.GUIDKey = value; } }
        public string GUIDKey { get { return Record.GUIDKey; } set { Record.GUIDKey = value; } }
        public int SortOrder { get { return Record.SortOrder; } set { Record.SortOrder = value; } }
        public int PortalId { get { return Record.PortalId; } }
        public bool Exists { get { if (Record.ItemID <= 0) { return false; } else { return true; }; } }
        public string AppThemeFolder { get { return Record.GetXmlProperty("genxml/remote/apptheme"); } set { Record.SetXmlProperty("genxml/remote/apptheme", value); } }
        public string AppThemeVersion { get { return Record.GetXmlProperty("genxml/remote/appthemeversion"); } set { Record.SetXmlProperty("genxml/remote/appthemeversion", value); } }
        public string AppThemeViewFolder { get { if (Record.GetXmlProperty("genxml/remote/appthemeview") == "") return AppThemeFolder; else return Record.GetXmlProperty("genxml/remote/appthemeview"); } set { Record.SetXmlProperty("genxml/remote/appthemeview", value); } }
        public string AppThemeViewVersion { get { if (Record.GetXmlProperty("genxml/remote/appthemeviewversion") == "") return AppThemeVersion; else return Record.GetXmlProperty("genxml/remote/appthemeviewversion"); } set { Record.SetXmlProperty("genxml/remote/appthemeviewversion", value); } }
        public string ModuleName { get { return Record.GetXmlProperty("genxml/remote/modulename"); } set { Record.SetXmlProperty("genxml/remote/modulename", value); } }
        public string DataRef { get { if (Record.GetXmlProperty("genxml/remote/dataref") == "") return ModuleRef; else return Record.GetXmlProperty("genxml/remote/dataref"); } set { Record.SetXmlProperty("genxml/remote/dataref", value); } }
        public string SiteKey { get { return Record.GetXmlProperty("genxml/remote/sitekey"); } set { Record.SetXmlProperty("genxml/remote/sitekey", value); } }
        public string Organisation { get { return Record.GetXmlProperty("genxml/remote/org"); } }
        public string TemplateSelected { get { return Record.GetXmlProperty("genxml/remote/template"); } }
        public string TemplateSelectedSettings { get { return Path.GetFileNameWithoutExtension(TemplateSelected) + "-Settings" + Path.GetExtension(TemplateSelected); } }

        
        #endregion

    }
}
