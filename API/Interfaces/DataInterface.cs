using Simplisity;
using System.Collections.Generic;

namespace DNNrocketAPI.Interfaces
{

    public abstract class DNNrocketCtrlInterface
    {
        public abstract int GetListCount(int portalId, int moduleId, string typeCode, string sqlSearchFilter = "", string lang = "", string tableName = "DNNrocket");
        public abstract List<SimplisityInfo> GetList(int portalId, int moduleId, string typeCode, string sqlSearchFilter = "", string lang = "", string sqlOrderBy = "", int returnLimit = 0, int pageNumber = 0, int pageSize = 0, int recordCount = 0, string tableName = "DNNrocket");
        public abstract SimplisityInfo GetInfo(int itemId, string lang, string tableName = "DNNrocket");
        public abstract SimplisityRecord GetRecord(int itemId, string tableName = "DNNrocket");
        public abstract void Delete(int itemId, string tableName = "DNNrocket");
        public abstract void CleanData(string tableName = "DNNrocket");
        public abstract void DeleteAllData(string tableName);
    }
}
