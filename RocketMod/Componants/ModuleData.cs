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
        private int _moduleid;
        private bool _isList;

        public SimplisityInfo ConfigInfo;
        public SimplisityInfo HeaderInfo;


        public ModuleData(int moduleId)
        {
            _moduleid = moduleId;
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
                _dataList = objCtrl.GetList(-1, _moduleid, "DATA", filter, DNNrocketUtils.GetCurrentCulture(), "", 0, 0, 0, 0);
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

        public void SaveConfig(SimplisityInfo postInfo)
        {
            var objCtrl = new DNNrocketController();
            var info = objCtrl.SaveData("rocketmod_" + _moduleid, "CONFIG", postInfo, -1, _moduleid);
        }

        #endregion

        #region "properties"

        public bool ConfigExists { get { return _configExists; } }
        public int ModuleId { get {return _moduleid;} }

        public bool IsList { get { return _isList; } set { _isList = value; } }
        

        public List<SimplisityInfo> List
        {
            get { return _dataList; }
        }

        #endregion



        #region "Private"


        #endregion




    }

}
