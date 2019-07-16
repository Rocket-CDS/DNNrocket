using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Xml;

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
        }

        public void AddListItem(string listName)
        {
            foreach (var s in SimplisityInfoList)
            {
                s.Value.AddListItem(listName);
            }
        }

        public void RemoveListRowByIndex(string listName, int index)
        {
            foreach (var s in SimplisityInfoList)
            {
                if (s.Value.XMLDoc != null)
                {
                    s.Value.RemoveListItem(listName, index);
                }
            }

        }

        public void RemovedDeletedListRecords(string listName, SimplisityInfo databaseInfo, SimplisityInfo postInfo)
        {
            // check against new data and find removed list items.
            var removeList = new Dictionary<int, string>();
            var list = databaseInfo.GetList(listName);
            foreach (var s in list)
            {
                var keyref = s.GetXmlPropertyInt("genxml/index");
                if (postInfo.GetListItem(listName, "/genxml/index", keyref.ToString()) == null)
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
                RemoveListRowByIndex(r.Value, r.Key);
            }

        }


        public void SortListRecordsOnSave(string listName, SimplisityInfo postInfo, string editlang)
        {
            // find new sort list
            var newsortorder = new Dictionary<int, SimplisityInfo>();
            var l = postInfo.GetList(listName);
            foreach (var s in l)
            {
                var index = s.GetXmlPropertyInt("genxml/index");
                if (!newsortorder.ContainsKey(index)) newsortorder.Add(index, s);
            }

            // Update ALL langauge records.
            foreach (var listInfoItem in SimplisityInfoList)
            {
                var saveInfo = (SimplisityInfo)postInfo.Clone();

                if (editlang != listInfoItem.Value.Lang)
                {
                   
                    // If it's not the same langauge, update the data with the listItem.
                    saveInfo.RemoveLangRecord();
                    saveInfo.SetLangRecord(listInfoItem.Value.GetLangRecord());

                    // resequance the other language list, by rebuilding from sorted GetList.
                    saveInfo.RemoveList(listName);
                    foreach (var s in newsortorder)
                    {
                        var sInfo = GetListItemByIndex(listInfoItem.Value, listName, s.Key.ToString());
                        saveInfo.AddListItem(listName, sInfo);
                    }

                }
                AddSimplisityInfo(saveInfo, listInfoItem.Value.Lang);
            }

        }

        public SimplisityInfo GetListItemByIndex(SimplisityInfo sInfo, string listName, string index)
        {
            var rtnList = new List<SimplisityInfo>();

            if (sInfo.XMLDoc != null)
            {
                var lp = 1;
                var listRecords = sInfo.XMLDoc.SelectNodes("genxml/" + listName + "/*");
                if (listRecords != null)
                {
                    foreach (XmlNode i in listRecords)
                    {
                        var baseindex = i.SelectSingleNode("index");
                        if (baseindex != null)
                        {
                            var nbi = new SimplisityInfo();
                            nbi.XMLData = i.OuterXml;
                            nbi.TypeCode = "LIST";
                            nbi.GUIDKey = listName;
                            var listXmlNode = sInfo.GetXmlNode("genxml/lang/genxml/" + listName + "/genxml[index = " + baseindex.InnerText + "]");
                            nbi.SetLangXml("<genxml>" + listXmlNode + "</genxml>");
                            rtnList.Add(nbi);
                            lp += 1;
                        }
                    }
                }
            }

            foreach (var i in rtnList)
            {
                if (index == i.GetXmlProperty("genxml/index"))
                {
                    return i;
                }
            }

            return null;
        }



        public ConcurrentDictionary<string, SimplisityInfo> SimplisityInfoList { get; }


    }
}
