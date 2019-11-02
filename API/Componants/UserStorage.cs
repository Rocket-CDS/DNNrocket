using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Componants
{
    public class UserStorage
    {
        private const string _tableName = "DNNRocket";
        private const string _entityTypeCode = "USERSTORAGE";
        private DNNrocketController _objCtrl;
        private string _guidKey;
        public UserStorage(int userId = -1)
        {
            UserId = userId;
            if (userId <= 0) UserId = UserUtils.GetCurrentUserId();

            _guidKey = _entityTypeCode + "*" + UserId;
            _objCtrl = new DNNrocketController();

            Record = (SimplisityRecord)CacheUtils.GetCache(_guidKey);
            if (Record == null)
            {
                Record = _objCtrl.GetRecordByGuidKey(-1, -1, _entityTypeCode, _guidKey, UserId.ToString());
                if (Record == null)
                {
                    Record = new SimplisityRecord();
                    Record.PortalId = -1;
                    Record.ModuleId = -1;
                    Record.TypeCode = _entityTypeCode;
                    Record.GUIDKey = _guidKey;
                    Record.UserId = UserId;
                    _objCtrl.SaveRecord(Record, _tableName);
                }
                CacheUtils.SetCache(_guidKey, Record);
            }
        }

        public void Save()
        {
            _objCtrl.SaveRecord(Record);
            CacheUtils.SetCache(_guidKey, Record);
        }
        public void Delete()
        {
            _objCtrl.Delete(Record.ItemID);
            ClearCache();
        }
        public void ClearCache()
        {
            CacheUtils.RemoveCache(_guidKey);
        }

        public void Set(string nodename,string value)
        {
            Record.SetXmlProperty("genxml/hidden/" + nodename, value);
            Save();
        }
        public string Get(string nodename)
        {
            return Record.GetXmlProperty("genxml/hidden/" + nodename);
        }


        public int UserId { get; set; }
        public SimplisityRecord Record { get; set; } 


    }
}
