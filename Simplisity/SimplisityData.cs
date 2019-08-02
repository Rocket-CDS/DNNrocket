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

        public void RemovedDeletedListRecords(string listName, SimplisityInfo databaseInfo, SimplisityInfo postInfo)
        {
            var newsortorder = new Dictionary<int, SimplisityInfo>();
            var newupdate = new List<SimplisityInfo>();

            var l = postInfo.GetList(listName);
            foreach (var s in l)
            {
                var index = s.GetXmlPropertyInt("genxml/index");
                if (!newsortorder.ContainsKey(index)) newsortorder.Add(index, s);
            }

            foreach (var listInfoItem in SimplisityInfoList)
            {
                if (postInfo.Lang != listInfoItem.Value.Lang)
                {
                    var l2 = listInfoItem.Value.GetList(listName);
                    foreach (var sInfo in l2)
                    {
                        var index = sInfo.GetXmlPropertyInt("genxml/index");
                        if (!newsortorder.ContainsKey(index))
                        {
                            sInfo.RemoveXmlNode("genxml/" + listName + "/genxml[index = " + index + "]");
                            sInfo.RemoveXmlNode("genxml/lang/genxml/" + listName + "/genxml[index = " + index + "]");
                        }
                        newupdate.Add(sInfo);
                    }
                }

                //update
                foreach (var sInfo in newupdate)
                {
                    AddSimplisityInfo(sInfo, sInfo.Lang);
                }

            }
        }

        public Dictionary<int, SimplisityInfo> GetListInOrder(SimplisityInfo sInfo, string listName)
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


        public void SortListRecordsOnSave(string listName, SimplisityInfo postInfo, string editlang)
        {
            if (SimplisityInfoList.Count >= 2) // no sort needed for 1 langauge
            {
                // find new sort list
                var newsortorder = GetListInOrder(postInfo, listName);

                var newupdate = new List<SimplisityInfo>();
                // calc all languages.
                foreach (var listInfoItem in SimplisityInfoList)
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
                        foreach (var s in newsortorder)
                        {
                            var sInfo = s.Value;
                            if (oldsortorder.ContainsKey(s.Key)) sInfo = oldsortorder[s.Key];
                            saveInfo.AddListItem(listName, sInfo);
                        }
                    }
                    newupdate.Add(saveInfo);
                }

                //update
                foreach (var sInfo in newupdate)
                {
                    AddSimplisityInfo(sInfo, sInfo.Lang);
                }
            }
        }

        public SimplisityInfo GetListItemByIndex(SimplisityInfo postInfo, SimplisityInfo sInfo, string listName, string index)
        {
            if (sInfo.XMLDoc != null && postInfo.XMLDoc != null)
            {
                var indexlang = 0;
                var nbi = new SimplisityInfo();

                var nodList = postInfo.XMLDoc.SelectNodes("genxml/" + listName + "/genxml");
                foreach (XmlNode nod in nodList)
                {
                    indexlang += 1;
                    if (nod.SelectSingleNode("index") != null && nod.SelectSingleNode("index").InnerText == index)
                    {
                        nbi.XMLData = nod.OuterXml;
                        var listXmlNode = sInfo.GetXmlNode("genxml/lang/genxml/" + listName + "/genxml[" + indexlang + "]");
                        nbi.SetLangXml("<genxml>" + listXmlNode + "</genxml>");
                    }
                }

                nbi.TypeCode = "LIST";
                nbi.GUIDKey = listName;
                return nbi;
            }
            return null;
        }



        public ConcurrentDictionary<string, SimplisityInfo> SimplisityInfoList { get; }


    }
}
