using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Componants
{
    public class SortLists
    {
        private DNNrocketController _objCtrl;
        private string _tableName;
        private int _systemid;
        private SimplisityData _simplisityData;
        private bool _debugmode;
        public SortLists(SimplisityInfo sInfo, string tableName = "DNNrocket", bool debugmode = false)
        {
            _debugmode = debugmode;
            _objCtrl = new DNNrocketController();
            _simplisityData = new SimplisityData();
            _systemid = sInfo.SystemId;
            _tableName = tableName;

            if (_debugmode)
            {
                var debugStr = sInfo.ToXmlItem();
                FileUtils.SaveFile(DNNrocketUtils.HomeDirectory() + "\\sortlist_startinfo.xml", debugStr);
            }

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
                    _simplisityData.AddSimplisityInfo(s, cultureCode);
                }
            }

            if (!_simplisityData.SimplisityInfoList.ContainsKey(sInfo.Lang))
            {
                // new lang record, so add it to list
                _simplisityData.AddSimplisityInfo(sInfo, sInfo.Lang);
            }

            // loop on all lists
            var allLists = sInfo.GetLists();
            foreach (var listName in allLists)
            {
                _simplisityData = SortListRecordsOnSave(_simplisityData, listName, sInfo, sInfo.Lang);
            }

        }

        public void Save()
        {
            var debugStr = "<root>";
            foreach (var listItem in _simplisityData.SimplisityInfoList)
            {
                if (_debugmode) debugStr += listItem.Value.ToXmlItem();
                _objCtrl.SaveData(listItem.Value, _systemid, _tableName);
            }
            if (_debugmode)
            {
                debugStr += "</root>";
                FileUtils.SaveFile(DNNrocketUtils.HomeDirectory() + "\\sortlist.xml", debugStr);
            }
        }

        private static SimplisityData SortListRecordsOnSave(SimplisityData simplisityData, string listName, SimplisityInfo postInfo, string editlang)
        {
            if (simplisityData.SimplisityInfoList.Count >= 2) // no sort needed for 1 langauge
            {
                // find new sort list
                var newsortorder = GetListInOrder(postInfo, listName);

                var newupdate = new List<SimplisityInfo>();
                // calc all languages.
                foreach (var listInfoItem in simplisityData.SimplisityInfoList)
                {
                    var saveInfo = (SimplisityInfo)postInfo.Clone();
                    saveInfo.Lang = listInfoItem.Key; // make sure we get the correct langauge on the record.

                    saveInfo.RemoveLangRecord();
                    if (editlang == listInfoItem.Key)
                    {
                        saveInfo.SetLangRecord(postInfo.GetLangRecord());
                    }
                    else
                    {
                        var oldsortorder = GetListInOrder(listInfoItem.Value, listName);
                        saveInfo.SetLangRecord(listInfoItem.Value.GetLangRecord());
                        saveInfo.RemoveList(listName);
                        var idx = 1;
                        foreach (var s in newsortorder)
                        {
                            var sInfo = s.Value;
                            if (oldsortorder.ContainsKey(s.Key)) sInfo = oldsortorder[s.Key];
                            saveInfo.AddListItem(listName, sInfo);
                            sInfo.SetXmlProperty("genxml/index", idx.ToString());
                            idx += 1;
                        }
                    }
                    newupdate.Add(saveInfo);
                }

                //update
                foreach (var sInfo in newupdate)
                {
                    simplisityData.AddSimplisityInfo(sInfo, sInfo.Lang);
                }
            }
            return simplisityData;
        }

        private static Dictionary<int, SimplisityInfo> GetListInOrder(SimplisityInfo sInfo, string listName)
        {
            var rtnsortorder = new Dictionary<int, SimplisityInfo>();
            var l = sInfo.GetList(listName);
            foreach (var s in l)
            {
                var index = s.GetXmlPropertyInt("genxml/index");
                if (!rtnsortorder.ContainsKey(index)) rtnsortorder.Add(index, s);
            }
            return rtnsortorder;
        }


    }

}
