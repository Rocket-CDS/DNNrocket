using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Componants
{
    public class SortLists : SimplisityData
    {
        private string _editlang;
        private DNNrocketController _objCtrl;
        private string _tableName;
        private int _systemid;
        public SortLists(SimplisityInfo sInfo, string tableName = "DNNrocket")
        {
            _objCtrl = new DNNrocketController();

            _editlang = DNNrocketUtils.GetEditCulture();
            _systemid = sInfo.SystemId;
            _tableName = tableName;

            var cultureList = new List<string>();
            var ls = DNNrocketUtils.GetCultureCodeList();
            foreach (var s in ls)
            {
                if (!cultureList.Contains(s)) cultureList.Add(s);
            }

            foreach (var cultureCode in cultureList)
            {
                var s = _objCtrl.GetData(sInfo.TypeCode, sInfo.ItemID, cultureCode, _systemid, sInfo.ModuleId, false, _tableName);
                if (s != null)
                {
                    AddSimplisityInfo(s, cultureCode);
                }
            }

            if (!SimplisityInfoList.ContainsKey(sInfo.Lang))
            {
                // new lang record, so add it to list
                AddSimplisityInfo(sInfo, sInfo.Lang);
            }

            // loop on all lists
            var allLists = sInfo.GetLists();
            foreach (var listName in allLists)
            {
                SortListRecordsOnSave(listName, sInfo, _editlang);
            }

        }

        public void Save()
        {
            foreach (var listItem in SimplisityInfoList)
            {
                _objCtrl.SaveData(listItem.Value, _systemid, _tableName);
            }
        }

    }

}
