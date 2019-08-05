using DNNrocketAPI;
using DNNrocketAPI.Componants;
using RocketSettings;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace RocketMod
{

    public class ModuleData 
    {

        private int _systemid;
        private string _entityTypeCode;
        private string _langRequired;


        private SettingsData _settingData;
        private ConfigData _configData;
        private SimplisityInfo _headerInfo;
        private SimplisityInfo _auditInfo;
        private SimplisityInfo _currentRecord;

        public ModuleData(ConfigData configData, string langRequired = "")
        {
            _langRequired = langRequired;
            if (_langRequired == "") _langRequired =  DNNrocketUtils.GetCurrentCulture();
            _entityTypeCode = "ROCKETMOD";
            _systemid = configData.SystemId;
            _configData = configData;

            Populate();
        }

        public void DeleteData()
        {
            var objCtrl = new DNNrocketController();
            var info = objCtrl.GetData("moduleid" + ModuleId, "ROCKETMOD", _langRequired, -1, ModuleId, true);
            if (info != null) objCtrl.Delete(info.ItemID);

            ClearCache();
            if (_configData.Exists)
            {
                PopulateHeader();
            }
        }

        public void Populate()
        {
            if (_configData.Exists)
            {
                _settingData = new SettingsData(configData.TabId, configData.DataModuleId, _langRequired, _entityTypeCode);
                PopulateHeader();
                _currentRecord = _settingData.Info;
            }
            else
            {
                _currentRecord = new SimplisityInfo();
                _settingData = new SettingsData(configData.TabId, configData.DataModuleId, DNNrocketUtils.GetCurrentCulture(), _entityTypeCode, "settingsdata",true);
            }
        }


        #region "HEADER"

        public void PopulateHeader()
        {
            var objCtrl = new DNNrocketController();
            _headerInfo = objCtrl.GetData(_configData.ModuleRef, "HEADER", _langRequired, -1, ModuleId, true);
            if (_headerInfo == null)
            {
                _headerInfo = new SimplisityInfo();
                _headerInfo.ModuleId = ModuleId;
            }
        }

        public void DeleteHeader()
        {
            var objCtrl = new DNNrocketController();
            var info = objCtrl.GetData(_configData.ModuleRef, "HEADER", _langRequired, -1, ModuleId, true);
            if (info != null)
            {
                objCtrl.Delete(info.ItemID);
                ClearCache();
                if (_configData.Exists)
                {
                    PopulateHeader();
                }
            }
        }

        public void SaveHeader(SimplisityInfo postInfo)
        {
            //remove any params
            postInfo.RemoveXmlNode("genxml/postform");
            postInfo.RemoveXmlNode("genxml/urlparams");

            var objCtrl = new DNNrocketController();
            var info = objCtrl.SaveData(_configData.ModuleRef, "HEADER", postInfo, -1, ModuleId);
            ClearCache();
            if (_configData.Exists)
            {
                PopulateHeader();
            }
        }

        #endregion

        #region "AUDIT"

        public void PopulateAudit()
        {
            if (_currentRecord != null && _currentRecord.ItemID > 0 && _currentRecord.TypeCode == _entityTypeCode)
            {
                var objCtrl = new DNNrocketController();
                var auditRecord = objCtrl.GetByGuidKey(-1, ModuleId,"AUDIT", _currentRecord.GUIDKey);
                if (auditRecord == null)
                {
                    _auditInfo = new SimplisityInfo();
                    _auditInfo.ItemID = -1;
                    _auditInfo.GUIDKey = _currentRecord.GUIDKey;
                    _auditInfo.TypeCode = "AUDIT";
                    _auditInfo.ModuleId = ModuleId;
                    _auditInfo.PortalId = _currentRecord.PortalId;
                    _auditInfo.ParentItemId = _currentRecord.ItemID;
                    _auditInfo.SystemId = _currentRecord.SystemId;
                    objCtrl.SaveRecord(_auditInfo);
                }
                else
                {
                    _auditInfo = objCtrl.GetInfo(auditRecord.ItemID, _langRequired);
                }

            }

        }

        public void AddAudit()
        {
            if (_currentRecord != null && _currentRecord.ItemID > 0 && _auditInfo != null && _auditInfo.ItemID > 0)
            {
                var objCtrl = new DNNrocketController();
                _currentRecord.SetXmlProperty("genxml/hidden/auditdate", DateTime.Now.ToString("s"));
                _auditInfo.AddListItem("audit", _currentRecord);
                objCtrl.SaveRecord(_auditInfo);
                PopulateAudit();
            }
        }

        public List<SimplisityInfo> GetAuditList()
        {
            if (_auditInfo == null) return new List<SimplisityInfo>();
            return _auditInfo.GetList("audit");
        }

        #endregion

        #region "properties"

        public int PortalId { get { return _settingData.Info.PortalId; } }
        public int ModuleId { get {return configData.ModuleId; } }
        public int DataModuleId { get { return configData.DataModuleId; } }
        public int TabId { get { return configData.TabId; } }
        public int ItemId { get { return _settingData.Info.ItemID; } }
        public string CultureCode { get { return _langRequired; } }
        
        public SimplisityInfo CurrentRecord { get { return _currentRecord; } }
        public SimplisityInfo HeaderInfo { get { return _headerInfo; } }
        public SettingsData Data { get { return _settingData; } }
        public ConfigData configData { get { return _configData; } }        

        public string EntityTypeCode { get { return _entityTypeCode; } }        

        public List<SimplisityInfo> List
        {
            get { return _settingData.List; }
        }

        #endregion

        public void ClearCache()
        {
            CacheUtils.ClearAllCache();
        }




    }

}
