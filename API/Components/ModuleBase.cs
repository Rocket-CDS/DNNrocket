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
        private string _entityTypeCode;
        private string _cacheKey;
        private SimplisityRecord _record;
        public ModuleBase(int portalId, string moduleRef, int moduleid = -1, int tabid = -1)
        {
            _record = new SimplisityRecord();
            _entityTypeCode = "MODSETTINGS";
            _cacheKey = portalId + moduleRef + _entityTypeCode;
            _objCtrl = new DNNrocketController();
            _record = (SimplisityRecord)CacheUtils.GetCache(_cacheKey, moduleRef);
            if (_record == null)
            {
                _record = _objCtrl.GetRecordByGuidKey(portalId, -1, _entityTypeCode, moduleRef, "", _tableName);
                if (_record == null)
                {
                    _record = new SimplisityRecord();
                    _record.PortalId = portalId;
                    _record.GUIDKey = moduleRef;
                    _record.TypeCode = _entityTypeCode;
                }
                else
                {
                    CacheUtils.SetCache(_cacheKey, _record, ModuleRef);
                }
            }
            // Outside initial setup, incase of changes in the CMS.
            _record.ModuleId = moduleid;
            TabId = tabid;
        }
        public int Save(SimplisityInfo paramInfo)
        {
            _record.RemoveXmlNode("genxml/settings");
            _record = DNNrocketUtils.UpdateSimplsityRecordFields(_record, paramInfo, "genxml/settings/*");
            return Update();
        }
        public int Update()
        {
            _record = _objCtrl.SaveRecord(_record, _tableName);
            CacheUtils.RemoveCache(_cacheKey);
            return _record.ItemID;
        }
        public string GetSetting(string key)
        {
            return _record.GetXmlProperty("genxml/settings/" + key);
        }
        public int GetSettingInt(string key)
        {
            return _record.GetXmlPropertyInt("genxml/settings/" + key);
        }
        public bool GetSettingBool(string key)
        {
            return _record.GetXmlPropertyBool("genxml/settings/" + key);
        }
        public void SetSetting(string key, string value)
        {
            _record.SetXmlProperty("genxml/settings/" + key.ToLower(), value);
        }
        #region "properties"

        public string EntityTypeCode { get { return _entityTypeCode; } }
        public SimplisityRecord Record { get { return _record; } set { _record = value; } }
        public int ModuleId { get { return _record.ModuleId; } set { _record.ModuleId = value; } }
        public int XrefItemId { get { return _record.XrefItemId; } set { _record.XrefItemId = value; } }
        public int ParentItemId { get { return _record.ParentItemId; } set { _record.ParentItemId = value; } }
        public int ItemId { get { return _record.ItemID; } set { _record.ItemID = value; } }
        public string ModuleRef { get { return _record.GUIDKey; } set { _record.GUIDKey = value; } }
        public string GUIDKey { get { return _record.GUIDKey; } set { _record.GUIDKey = value; } }
        public int SortOrder { get { return _record.SortOrder; } set { _record.SortOrder = value; } }
        public int PortalId { get { return _record.PortalId; } }
        public bool Exists { get { if (_record.ItemID <= 0) { return false; } else { return true; }; } }
        public int TabId { get { return _record.GetXmlPropertyInt("genxml/data/tabid"); } set { _record.SetXmlProperty("genxml/data/tabid", value.ToString()); } }
        public string AppThemeAdminFolder { get { return _record.GetXmlProperty("genxml/data/appthemeadminfolder"); } set { _record.SetXmlProperty("genxml/data/appthemeadminfolder", value); } }
        public bool HasAppThemeAdmin { get { if (_record.GetXmlProperty("genxml/data/appthemeadminfolder") == "") return false; else return true; } }
        public string AppThemeAdminVersion { get { return _record.GetXmlProperty("genxml/data/appthemeadminversion"); } set { _record.SetXmlProperty("genxml/data/appthemeadminversion", value); } }
        public bool HasAppThemeAdminVersion { get { if (_record.GetXmlProperty("genxml/data/appthemeadminversion") == "") return false; else return true; } }
        public string AppThemeViewFolder { get { return AppThemeAdminFolder; } }
        public string AppThemeViewVersion { get { return AppThemeAdminVersion; } }
        public string DataRef { get { if (_record.GetXmlProperty("genxml/settings/dataref") == "") return ModuleRef; else return _record.GetXmlProperty("genxml/settings/dataref"); } set { _record.SetXmlProperty("genxml/settings/dataref", value); } }
        public string ProjectName { get { return _record.GetXmlProperty("genxml/data/projectname"); } set { _record.SetXmlProperty("genxml/data/projectname", value); } }
        public bool HasProject { get { if (_record.GetXmlProperty("genxml/data/projectname") == "") return false; else return true; } }
        public bool InjectJQuery { get { return _record.GetXmlPropertyBool("genxml/settings/injectjquery"); } set { _record.SetXmlProperty("genxml/settings/injectjquery", value.ToString()); } }
        public bool DisableCache { get { return _record.GetXmlPropertyBool("genxml/settings/disablecache"); } set { _record.SetXmlProperty("genxml/settings/disablecache", value.ToString()); } }
        public bool DisableHeader { get { return _record.GetXmlPropertyBool("genxml/settings/disableheader"); } set { _record.SetXmlProperty("genxml/settings/disableheader", value.ToString()); } }
        public bool SecureSave { get { return _record.GetXmlPropertyBool("genxml/settings/securesave"); } set { _record.SetXmlProperty("genxml/settings/securesave", value.ToString()); } }
        public string Name { get { return _record.GetXmlProperty("genxml/settings/name"); } set { _record.SetXmlProperty("genxml/settings/name", value); } }
        public string SystemKey { get { return _record.GetXmlProperty("genxml/systemkey"); } set { _record.SetXmlProperty("genxml/systemkey", value); } }
        public bool IsSatellite { get { if (_record.GUIDKey == _record.GetXmlProperty("genxml/settings/dataref") || _record.GetXmlProperty("genxml/settings/dataref") == "") return false; else return true; } }
        /// <summary>
        /// Gets or sets a value indicating whether ECOMode is activated for the module. 
        /// The module settings should have a value with the xpath of "genxml/settings/ecomode", default is true.
        /// </summary>
        public bool ECOMode { get {
                if (_record.GetXmlProperty("genxml/settings/ecomode") == "") return true;
                return _record.GetXmlPropertyBool("genxml/settings/ecomode"); 
            } set { _record.SetXmlProperty("genxml/settings/ecomode", value.ToString()); } }
        #endregion

    }
}
