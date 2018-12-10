using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Interfaces
{
    public class DataInterface
    {

        public abstract class NBrightDataCtrlInterface
        {
            public abstract List<SimplisityInfo> GetList(int portalId, int moduleId, string typeCode, string sqlSearchFilter = "", string sqlOrderBy = "", int returnLimit = 0, int pageNumber = 0, int pageSize = 0, int recordCount = 0, string lang = "");
            public abstract int GetListCount(int portalId, int moduleId, string typeCode, string sqlSearchFilter = "", string lang = "");
            public abstract SimplisityInfo Get(int itemId, string lang = "");
            public abstract SimplisityInfo GetData(int itemId);
            public abstract int Update(SimplisityInfo objInfo);
            public abstract void Delete(int itemId);
            public abstract void CleanData();
        }
    }
}
