using DNNrocketAPI;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocketMod
{

    public class ModuleData
    {
        private List<SimplisityInfo> _dataList;
        private bool _configExists;
        private int _tabid;
        private int _moduleid;
        private int _selecteditemid;
        private bool _isList;
        
        public SimplisityInfo ConfigInfo;
        public SimplisityInfo HeaderInfo;


        public ModuleData(int tabId, int moduleId, int selecteditemid)
        {
            _tabid = tabId;
            _moduleid = moduleId;
            _selecteditemid = selecteditemid;
            _dataList = new List<SimplisityInfo>();
            
            PopulateConfig();
            if (_configExists)
            {
                PopulateList();
            }
        }

        #region "Data"



        public void PopulateList()
        {
            if (_moduleid > 0)
            {
                var objCtrl = new DNNrocketController();
                var filter = "";
                if (_selecteditemid > 0)
                {
                    filter = " and R1.ItemId = " + _selecteditemid + " ";
                }
                _dataList = objCtrl.GetList(-1, _moduleid, "DATA", filter, DNNrocketUtils.GetEditCulture(), "", 0, 0, 0, 0);
            }
        }

        public void AddNew()
        {
            var info = new SimplisityInfo();
            info.ItemID = -1;
            info.PortalId = DNNrocketUtils.GetPortalId();
            info.Lang = DNNrocketUtils.GetEditCulture();
            info.TypeCode = "DATA";
            info.ModuleId = _moduleid;

            var objCtrl = new DNNrocketController();
            var newInfo = objCtrl.SaveData(info, -1);
            _selecteditemid = newInfo.ItemID;
            PopulateList();
        }

        public void DeleteData()
        {
            if (_moduleid > 0)
            {
                if (_selecteditemid > 0)
                {
                    DeleteCurrentRecord();
                }
                else
                {
                    var objCtrl = new DNNrocketController();
                    var filter = "";
                    _dataList = objCtrl.GetList(-1, _moduleid, "DATA", filter, DNNrocketUtils.GetEditCulture(), "", 0, 0, 0, 0);
                    foreach (var i in _dataList)
                    {
                        objCtrl.Delete(i.ItemID);
                    }
                    _dataList = objCtrl.GetList(-1, _moduleid, "DATA", filter, DNNrocketUtils.GetEditCulture(), "", 0, 0, 0, 0);
                    PopulateList();
                }
            }
        }

        public void DeleteCurrentRecord()
        {
            if (_moduleid > 0)
            {
                var objCtrl = new DNNrocketController();
                objCtrl.Delete(_selecteditemid);
                _selecteditemid = 0;
                PopulateList();
            }
        }

        #endregion

        #region "CONFIG"

        public void PopulateConfig()
        {
            var objCtrl = new DNNrocketController();
            ConfigInfo = objCtrl.GetData("rocketmod_" + _moduleid, "CONFIG", DNNrocketUtils.GetEditCulture(), -1, _moduleid, true);
            if (ConfigInfo == null)
            {
                _configExists = false;
                ConfigInfo = new SimplisityInfo();
                ConfigInfo.ModuleId = _moduleid;
            }
            else
            {
                if (ConfigInfo.XMLDoc.SelectNodes("genxml/*").Count <= 1) // <lang> node will be created for new record.
                {
                    _configExists = false;
                }
                else
                {
                    _configExists = true;
                }
            }
        }

        public void DeleteConfig()
        {
            var objCtrl = new DNNrocketController();
            var info = objCtrl.GetData("rocketmod_" + _moduleid, "CONFIG", DNNrocketUtils.GetCurrentCulture(), -1, _moduleid, true);
            if (info != null)
            {
                objCtrl.Delete(info.ItemID);
                PopulateConfig();
                if (_configExists)
                {
                    PopulateList();
                }
            }
        }

        public void SaveConfig(SimplisityInfo postInfo, bool isList)
        {
            postInfo.SetXmlProperty("genxml/hidden/islist", "True");
            var objCtrl = new DNNrocketController();
            var info = objCtrl.SaveData("rocketmod_" + _moduleid, "CONFIG", postInfo, -1, _moduleid);
            PopulateConfig();
        }

        #endregion

        #region "properties"

        public bool ConfigExists { get { return _configExists; } }
        public int ModuleId { get {return _moduleid;} }
        public int TabId { get { return _tabid; } }
        public int SelectedItemId { get { return _selecteditemid; } }

        public bool IsList { get { return ConfigInfo.GetXmlPropertyBool("genxml/hidden/islist"); }}
        

        public List<SimplisityInfo> List
        {
            get { return _dataList; }
        }

        #endregion



        #region "Private"


        #endregion




    }

}
