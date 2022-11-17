using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Components
{
    /// <summary>
    /// The UserParams class saves user data to Cache.
    /// </summary>
    public class UserParams
    {
        private const string _entityTypeCode = "UserParams";
        private string _guidKey;
        private bool _useDB;
        private const string _tableName = "DNNrocketTemp";
        private DNNrocketController _objCtrl;

        public UserParams(int userId)
        {
            UserId = userId;
            if (userId <= 0) UserId = UserUtils.GetCurrentUserId();
            initUserParams(UserId.ToString(), true);
        }
        public UserParams(string browserSessionId)
        {
            initUserParams(browserSessionId, false);
        }
        /// <summary>
        /// Setup user session class to keep track of session
        /// </summary>
        /// <param name="browserSessionId"></param>
        /// <param name="saveToDB">ONLY save to DB if the UserId is used as a sessionid.  Stop DB filling.</param>
        private void initUserParams(string browserSessionId, bool useDB = false)
        {
            _objCtrl = new DNNrocketController();
            _useDB = useDB;
            _guidKey = _entityTypeCode + "*" + UserUtils.GetCurrentUserId() + "*" + browserSessionId;

            Record = new SimplisityRecord();

            // only put into memeory for users in Rocket Roles.
            if (UserUtils.IsEditor())
            {
                Record = (SimplisityRecord)CacheUtils.GetCache(_guidKey);
                if (Record == null)
                {
                    if (_useDB)
                    {
                        Record = _objCtrl.GetRecordByGuidKey(-1, -1, _entityTypeCode, _guidKey, UserId.ToString(), _tableName);
                    }
                    if (Record == null)
                    {
                        Record = new SimplisityRecord();
                        Record.PortalId = -1;
                        Record.ModuleId = -1;
                        Record.TypeCode = _entityTypeCode;
                        Record.GUIDKey = _guidKey;
                        Record.UserId = UserId;
                    }
                }

                Save();
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
        public void Save()
        {
            if (UserUtils.IsEditor())
            {
                if (_useDB)
                {
                    Record.ItemID = _objCtrl.Update(Record, _tableName);
                }                                       
                CacheUtils.SetCache(_guidKey, Record);
            }
        }
        public void Update()
        {
            Save();
        }
        public void Delete()
        {
            if (_useDB && Record != null) _objCtrl.Delete(Record.ItemID);
            ClearCache();
        }
        public void ClearCache()
        {
            CacheUtils.RemoveCache(_guidKey);
        }
        public void Set(string nodename,string value, string systemKey = "", System.TypeCode DataTyp = System.TypeCode.String)
        {
            if (systemKey != "") nodename = systemKey + "-" + nodename;
            Record.SetXmlProperty("genxml/hidden/" + nodename, value, DataTyp);
            Save();
        }
        public string Get(string nodename, string systemKey = "")
        {
            if (systemKey != "") nodename = systemKey + "-" + nodename;
            return Record.GetXmlProperty("genxml/hidden/" + nodename);
        }
        public int GetInt(string nodename, string systemKey = "")
        {
            if (systemKey != "") nodename = systemKey + "-" + nodename;
            return Record.GetXmlPropertyInt("genxml/hidden/" + nodename);
        }
        public bool GetBool(string nodename, string systemKey = "")
        {
            if (systemKey != "") nodename = systemKey + "-" + nodename;
            return Record.GetXmlPropertyBool("genxml/hidden/" + nodename);
        }
        public double GetDouble(string nodename, string systemKey = "")
        {
            if (systemKey != "") nodename = systemKey + "-" + nodename;
            return Record.GetXmlPropertyDouble("genxml/hidden/"  + nodename);
        }
        public DateTime GetDate(string nodename, string systemKey = "")
        {
            if (systemKey != "") nodename = systemKey + "-" + nodename;
            return Record.GetXmlPropertyDate("genxml/hidden/" + nodename);
        }

        public int ModuleId { get; set; }

        public int UserId { get; set; }
        public SimplisityRecord Record { get; set; }
        public SimplisityRecord Storage { get; set; }


    }
}
