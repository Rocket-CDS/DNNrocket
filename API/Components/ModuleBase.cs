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
    public class ModuleBase
    {
        private DNNrocketController _objCtrl;
        private const string _tableName = "DNNrocket";
        private const string _entityTypeCode = "RMODSETTINGS";
        private string _cacheKey;
        public ModuleBase(int portalId, string moduleRef, int moduleid = -1, int tabid = -1)
        {
            _cacheKey = portalId + moduleRef + _entityTypeCode;
            _objCtrl = new DNNrocketController();
            Record = (SimplisityRecord)CacheUtils.GetCache(_cacheKey, moduleRef);
            if (Record == null)
            {
                Record = _objCtrl.GetRecordByGuidKey(portalId, -1, _entityTypeCode, moduleRef, "", _tableName);
                if (Record == null)
                {
                    Record = new SimplisityRecord();
                    Record.PortalId = portalId;
                    Record.GUIDKey = moduleRef;
                    Record.TypeCode = _entityTypeCode;
                }
            }
            // Outside initial setup, incase of changes in the CMS.
            Record.ModuleId = moduleid;
            TabId = tabid;
        }
        public int Save(SimplisityInfo paramInfo)
        {
            Record.RemoveXmlNode("genxml/settings");
            Record = DNNrocketUtils.UpdateSimplsityRecordFields(Record, paramInfo, "genxml/settings/*");
            return Update();
        }
        public int Update()
        {
            Record = _objCtrl.SaveRecord(Record, _tableName);
            CacheUtils.SetCache(_cacheKey, Record, ModuleRef);
            return Record.ItemID;
        }

        #region "properties"

        public string EntityTypeCode { get { return _entityTypeCode; } }
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
        public int TabId { get { return Record.GetXmlPropertyInt("genxml/data/tabid"); } set { Record.SetXmlProperty("genxml/data/tabid", value.ToString()); } }
        public string AppThemeAdminFolder { get { return Record.GetXmlProperty("genxml/data/appthemeadminfolder"); } set { Record.SetXmlProperty("genxml/data/appthemeadminfolder", value); } }
        public bool HasAppThemeAdmin { get { if (Record.GetXmlProperty("genxml/data/appthemeadminfolder") == "") return false; else return true; } }
        public string AppThemeAdminVersion { get { return Record.GetXmlProperty("genxml/data/appthemeadminversion"); } set { Record.SetXmlProperty("genxml/data/appthemeadminversion", value); } }
        public bool HasAppThemeAdminVersion { get { if (Record.GetXmlProperty("genxml/data/appthemeadminversion") == "") return false; else return true; } }
        public string AppThemeViewFolder { get { return Record.GetXmlProperty("genxml/data/appthemeviewfolder"); } set { Record.SetXmlProperty("genxml/data/appthemeviewfolder", value); } }
        public bool HasAppThemeView { get { if (Record.GetXmlProperty("genxml/data/appthemeviewfolder") == "") return false; else return true; } }
        public string AppThemeViewVersion { get { return Record.GetXmlProperty("genxml/data/appthemeviewversion"); } set { Record.SetXmlProperty("genxml/data/appthemeviewversion", value); } }
        public bool HasAppThemeViewVersion { get { if (Record.GetXmlProperty("genxml/data/appthemeviewversion") == "") return false; else return true; } }
        public string DataRef { get { if (Record.GetXmlProperty("genxml/data/dataref") == "") return ModuleRef; else return Record.GetXmlProperty("genxml/data/dataref"); } set { Record.SetXmlProperty("genxml/data/dataref", value); } }
        public string ProjectName { get { return Record.GetXmlProperty("genxml/data/projectname"); } set { Record.SetXmlProperty("genxml/data/projectname", value); } }
        public bool HasProject { get { if (Record.GetXmlProperty("genxml/data/projectname") == "") return false; else return true; } }
        public bool InjectJQuery { get { return Record.GetXmlPropertyBool("genxml/settings/injectjquery"); } set { Record.SetXmlProperty("genxml/settings/injectjquery", value.ToString()); } }
        public bool DisableCache { get { return Record.GetXmlPropertyBool("genxml/settings/disablecache"); } set { Record.SetXmlProperty("genxml/settings/disablecache", value.ToString()); } }
        public bool DisableHeader { get { return Record.GetXmlPropertyBool("genxml/settings/disableheader"); } set { Record.SetXmlProperty("genxml/settings/disableheader", value.ToString()); } }
        public bool SecureSave { get { return Record.GetXmlPropertyBool("genxml/settings/securesave"); } set { Record.SetXmlProperty("genxml/settings/securesave", value.ToString()); } }

        #endregion

    }
}
