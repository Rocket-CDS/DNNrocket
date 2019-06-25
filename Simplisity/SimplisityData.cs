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

        public void AddListItem(string listName, SimplisityInfo simplisityInfo)
        {
            foreach (var s in SimplisityInfoList)
            {
                s.Value.AddListItem(listName, simplisityInfo);
            }
            AddRecordKey(listName);
        }

        public void AddListItem(string listName)
        {
            foreach (var s in SimplisityInfoList)
            {
                s.Value.AddListItem(listName);
            }
            AddRecordKey(listName);
        }

        private bool AddRecordKey(string listName)
        {
            // add recordkey
            var recordkey = GeneralUtils.GetUniqueKey();
            var rtn = false;
            foreach (var s in SimplisityInfoList)
            {
                var listdata = s.Value.GetList(listName);
                var lp = listdata.Count;
                if (s.Value.GetXmlProperty("genxml/" + listName + "/genxml[" + lp + "]/key1") != recordkey)
                {
                    s.Value.SetXmlProperty("genxml/" + listName + "/genxml[" + lp + "]/key1", recordkey);
                    rtn = true;
                }
                if (s.Value.GetXmlProperty("genxml/lang/genxml/" + listName + "/genxml[" + lp + "]/key2") != recordkey)
                {
                    s.Value.SetXmlProperty("genxml/lang/genxml/" + listName + "/genxml[" + lp + "]/key2", recordkey);
                    rtn = true;
                }
            }
            return rtn;
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

        public void RemovedDeletedListRecords(string listName, SimplisityInfo databaseInfo, SimplisityInfo postInfo)
        {
            // check against new data and find removed list items.
            var removeList = new Dictionary<string, string>();
            var list = databaseInfo.GetList(listName);
            foreach (var s in list)
            {
                var keyref = s.GetXmlProperty("genxml/key1");
                if (postInfo.GetListItem(listName, "/genxml/key1", keyref) == null)
                {
                    if (!removeList.ContainsKey(keyref))
                    {
                        removeList.Add(keyref, listName);
                    }
                }
            }

            foreach (var r in removeList)
            {
                // delete removed list items from all langauges
                RemoveListRowByKey(r.Value, r.Key);
            }

        }

        public void SortListRecordsOnSave(string listName, SimplisityInfo postInfo, string editlang)
        {

            // Update ALL langauge records.
            foreach (var listItem in SimplisityInfoList)
            {
                var saveInfo = (SimplisityInfo)postInfo.Clone();
                if (editlang != listItem.Value.Lang)
                {
                    // If it's not the same langauge, update the data with the listItem.
                    saveInfo.RemoveLangRecord();
                    saveInfo.SetLangRecord(listItem.Value.GetLangRecord());

                    // resequance the other language list, by rebuilding from sorted GetList.
                    var l = saveInfo.GetList(listName);
                    saveInfo.RemoveList(listName);
                    foreach (var s in l)
                    {
                        saveInfo.AddListItem(listName, s);
                    }
                }
                AddSimplisityInfo(saveInfo, saveInfo.Lang);
            }

        }


        public ConcurrentDictionary<string, SimplisityInfo> SimplisityInfoList { get; }


    }
}
