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
                if (s.Value.GetXmlProperty("genxml/" + listName + "/genxml[" + lp + "]/key1") != recordkey)
                {
                    s.Value.SetXmlProperty("genxml/" + listName + "/genxml[" + lp + "]/key1", recordkey);
                }
                if (s.Value.GetXmlProperty("genxml/lang/genxml/" + listName + "/genxml[" + lp + "]/key2") != recordkey)
                {
                    s.Value.SetXmlProperty("genxml/lang/genxml/" + listName + "/genxml[" + lp + "]/key2", recordkey);
                }
            }

        }

        public void RemoveListRowByKey(string listName, string recordKey)
        {
            foreach (var s in SimplisityInfoList)
            {
                if (s.Value.XMLDoc != null)
                {
                    s.Value.RemoveXmlNode("genxml/" + listName + "/genxml[key1 = '" + recordKey + "']");
                    s.Value.RemoveXmlNode("genxml/lang/genxml/" + listName + "/genxml[key2 = '" + recordKey + "']");
                }
            }

        }

        public void SortListByCultureCode(string listName, string cultureCode)
        {
            // get correct list order, but using "cultureCode"
            var keyInfo = SimplisityInfoList[cultureCode];
            var keyListOrder = keyInfo.GetList(listName);

            foreach (var sPair in SimplisityInfoList)
            {
                if (sPair.Value.Lang != cultureCode)
                {
                    var storeList = (SimplisityInfo)sPair.Value.Clone();
                    sPair.Value.RemoveList(listName);
                    foreach (var keyRec in keyListOrder)
                    {
                        var listRowInfo = storeList.GetListItem(listName, "genxml/key1", keyRec.GetXmlProperty("genxml/key1"));
                        if (listRowInfo != null)
                        {
                            sPair.Value.AddListRow(listName, listRowInfo);
                        }
                    }
                }
            }

        }


        public ConcurrentDictionary<string, SimplisityInfo> SimplisityInfoList { get; }


    }
}
