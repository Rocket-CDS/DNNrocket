using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Simplisity
{
    public class SimplisityData
    {
        public SimplisityData()
        {
            SimplisityInfoList = new ConcurrentDictionary<string, SimplisityInfo>();
        }

        public void AddSimplisityInfo(SimplisityInfo simplisityInfo, string cultureCode)
        {
            if (cultureCode != "")
            {
                simplisityInfo.Lang = cultureCode;
                SimplisityInfoList.AddOrUpdate(simplisityInfo.Lang, simplisityInfo, (key, existingVal) => { return simplisityInfo; });
            }
        }

        public SimplisityInfo GetInfo(string cultureCode)
        {
            SimplisityInfo rtn;
            SimplisityInfoList.TryGetValue(cultureCode, out rtn);
            return rtn;
        }

        public void RemoveInfo(string cultureCode)
        {
            SimplisityInfo v;
            if (!SimplisityInfoList.TryRemove(cultureCode, out v))
            {
                // should not fail, but ignore if does.
            }
        }

        public void AddListRow(string listName, SimplisityInfo simplisityInfo)
        {
            foreach (var s in SimplisityInfoList)
            {
                s.Value.AddListRow(listName, simplisityInfo);
            }
            AddRecordKey(listName);
        }

        public void AddListRow(string listName)
        {
            foreach (var s in SimplisityInfoList)
            {
                s.Value.AddListRow(listName);
            }
            AddRecordKey(listName);
        }

        private void AddRecordKey(string listName)
        {
            // add recordkey
            var recordkey = GeneralUtils.GetUniqueKey();
            foreach (var s in SimplisityInfoList)
            {
                var listdata = s.Value.GetList(listName);
                var lp = listdata.Count;
                if (s.Value.GetXmlProperty("genxml/" + listName + "/genxml[" + lp + "]/recordkey") != recordkey)
                {
                    s.Value.SetXmlProperty("genxml/" + listName + "/genxml[" + lp + "]/recordkey", recordkey);
                }
            }

        }


        public void RemoveListRow(string listName, int index)
        {
            foreach (var s in SimplisityInfoList)
            {
                s.Value.RemoveListRow(listName, index);
            }
        }

        public void RemoveListRowByKey(string listName, string recordKey)
        {
            foreach (var s in SimplisityInfoList)
            {
                s.Value.RemoveListRowByKey(listName, recordKey);
            }
        }


        public ConcurrentDictionary<string, SimplisityInfo> SimplisityInfoList { get; }


    }
}
