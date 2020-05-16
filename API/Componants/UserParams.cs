using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Componants
{
    public class UserParams
    {
        private const string _tableName = "DNNRocketTemp";
        private const string _entityTypeCode = "UserParams";
        private DNNrocketController _objCtrl;
        private string _guidKey;
        public UserParams(int userId = -1)
        {
            UserId = userId;
            if (userId <= 0) UserId = UserUtils.GetCurrentUserId();

            _guidKey = _entityTypeCode + "*" + UserId;
            _objCtrl = new DNNrocketController();

            Record = (SimplisityRecord)CacheUtilsDNN.GetCache(_guidKey);
            if (Record == null)
            {
                Record = _objCtrl.GetRecordByGuidKey(-1, -1, _entityTypeCode, _guidKey, UserId.ToString(), _tableName);
                if (Record == null)
                {
                    Record = new SimplisityRecord();
                    Record.PortalId = -1;
                    Record.ModuleId = -1;
                    Record.TypeCode = _entityTypeCode;
                    Record.GUIDKey = _guidKey;
                    Record.UserId = UserId;

                    Record = Save();
                }
                CacheUtilsDNN.SetCache(_guidKey, Record);
            }
        }
        public string GetCommand(string systemKey)
        {
            return Get(systemKey + "-s-menu-cmd" + ModuleId);
        }
        public string GetInterfaceKey(string systemKey)
        {
            return Get(systemKey + "-s-menu-interfaceKey" + ModuleId);
        }
        public SimplisityInfo GetParamInfo(string systemKey)
        {
            var paramInfo = new SimplisityInfo();
            var xmlInfo = GeneralUtils.DeCode(Get(systemKey + "-s-menu-paraminfo" + ModuleId));
            if (xmlInfo != "") paramInfo.FromXmlItem(xmlInfo);
            return paramInfo;
        }
        public void Track(string systemKey, string paramCmd, SimplisityInfo _paramInfo, string interfaceKey)
        {
            Set(systemKey + "-s-menu-cmd" + ModuleId, paramCmd);
            Set(systemKey + "-s-menu-paraminfo" + ModuleId, GeneralUtils.EnCode(_paramInfo.ToXmlItem()));
            Set(systemKey + "-s-menu-interfaceKey" + ModuleId, interfaceKey);            
        }
        public void TrackClear(string systemKey)
        {
            Set(systemKey + "-s-menu-cmd" + ModuleId, "");
            Set(systemKey + "-s-menu-paraminfo" + ModuleId, "");
        }
        public SimplisityRecord Save()
        {
            if (UserId > 0)
            {
                CacheUtilsDNN.SetCache(_guidKey, Record);
                return _objCtrl.SaveRecord(Record, _tableName);
            }
            return new SimplisityRecord();
        }
        public SimplisityRecord Update()
        {
            return Save();
        }
        public void Delete()
        {
            _objCtrl.Delete(Record.ItemID, _tableName);
            ClearCache();
        }
        public void ClearCache()
        {
            CacheUtilsDNN.RemoveCache(_guidKey);
        }

        public void Set(string nodename,string value, string systemKey = "", System.TypeCode DataTyp = System.TypeCode.String)
        {
            Record.SetXmlProperty("genxml/hidden/" + systemKey + "-" + nodename, value, DataTyp);
            Save();
        }
        public string Get(string nodename, string systemKey = "")
        {
            return Record.GetXmlProperty("genxml/hidden/" + systemKey + "-" + nodename);
        }
        public int GetInt(string nodename, string systemKey = "")
        {
            return Record.GetXmlPropertyInt("genxml/hidden/" + systemKey + "-" + nodename);
        }
        public bool GetBool(string nodename, string systemKey = "")
        {
            return Record.GetXmlPropertyBool("genxml/hidden/" + systemKey + "-" + nodename);
        }
        public double GetDouble(string nodename, string systemKey = "")
        {
            return Record.GetXmlPropertyDouble("genxml/hidden/" + systemKey + "-" + nodename);
        }
        public DateTime GetDate(string nodename, string systemKey = "")
        {
            return Record.GetXmlPropertyDate("genxml/hidden/" + systemKey + "-" + nodename);
        }

        public int ModuleId { get; set; }

        public int UserId { get; set; }
        public SimplisityRecord Record { get; set; } 


    }
}
