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
                if (_debugmode)
                {
                    debugStr += listItem.Value.ToXmlItem();
                }
                _objCtrl.SaveData(listItem.Value, _systemid, _tableName);
            }
            if (_debugmode)
            {
                debugStr += "</root>";
                FileUtils.SaveFile(DNNrocketUtils.HomeDirectory() + "\\savesortlist.xml", debugStr);
            }
        }

        private SimplisityData SortListRecordsOnSave(SimplisityData simplisityData, string listName, SimplisityInfo postInfo, string editlang)
        {
            if (simplisityData.SimplisityInfoList.Count >= 2) // no sort needed for 1 langauge
            {
                // find new sort list
                var newsortorder = GetListInOrder(postInfo, listName);

                var newupdate = new List<SimplisityInfo>();
                // calc all languages.

                //var debugStr = "";
                //if (_debugmode)
                //{
                //    debugStr = FileUtils.ReadFile(DNNrocketUtils.HomeDirectory() + "\\debug_SortListRecordsOnSave.xml");
                //}
                //debugStr += "######------ start SortListRecordsOnSave() ----------" + Environment.NewLine;  // *** DEBUG ***
                //debugStr += "editlang: " + editlang + Environment.NewLine;  // *** DEBUG ***

                foreach (var listInfoItem in simplisityData.SimplisityInfoList)
                {
                    var saveInfo = (SimplisityInfo)postInfo.Clone();
                    saveInfo.Lang = listInfoItem.Key; // make sure we get the correct langauge on the record.

                    //debugStr += listInfoItem.Value.Lang + Environment.NewLine;  // *** DEBUG ***

                    if (editlang == listInfoItem.Key)
                    {
                        //debugStr += "Edit Language " + editlang + "==" + listInfoItem.Key + Environment.NewLine;  // *** DEBUG ***
                        var lp = 1;
                        foreach (var l in saveInfo.GetList(listName))
                        {
                            saveInfo.SetXmlProperty("genxml/" + listName + "/genxml[" + lp + "]/index", lp.ToString());
                        }
                    }
                    else
                    {
                        //debugStr += "Other Language " + editlang + "==" + listInfoItem.Key + Environment.NewLine;  // *** DEBUG ***

                        saveInfo.RemoveLangRecord();
                        var oldsortorder = GetListInOrder(listInfoItem.Value, listName);
                        saveInfo.SetLangRecord(listInfoItem.Value.GetLangRecord()); 
                        saveInfo.RemoveList(listName);
                        foreach (var s in newsortorder)
                        {
                            var sInfo = s.Value;
                            if (oldsortorder.ContainsKey(s.Key))
                            {
                                var lRecord = oldsortorder[s.Key].GetLangRecord();
                                sInfo.RemoveLangRecord();
                                sInfo.SetLangRecord(lRecord);
                            }

                            //debugStr += " key:" + s.Key + " altimagepath:" + sInfo.GetXmlProperty("genxml/lang/genxml/textbox/altimagepath") + Environment.NewLine;  // *** DEBUG ***

                            saveInfo.AddListItem(listName, sInfo);
                        }
                    }
                    newupdate.Add(saveInfo);
                    //debugStr += "-- SAVEINFO START " + saveInfo.Lang + " ---------------------------------------" + Environment.NewLine;  // *** DEBUG ***
                    //debugStr += " altimagepath[1]:" + saveInfo.GetXmlProperty("genxml/lang/genxml/imagelist/genxml[1]/textbox/altimagepath") + Environment.NewLine;  // *** DEBUG ***
                    //debugStr += " altimagepath[2]:" + saveInfo.GetXmlProperty("genxml/lang/genxml/imagelist/genxml[2]/textbox/altimagepath") + Environment.NewLine;  // *** DEBUG ***
                    //debugStr += "-- SAVEINFO END ---------------------------------------" + Environment.NewLine;  // *** DEBUG ***
                }

                //update
                foreach (var sInfo in newupdate)
                {
                    //debugStr += "-- Add AddSimplisityInfo START " + sInfo.Lang + "---------------------------------------" + Environment.NewLine;  // *** DEBUG ***
                    //debugStr += " altimagepath[1]:" + sInfo.GetXmlProperty("genxml/lang/genxml/imagelist/genxml[1]/textbox/altimagepath") + Environment.NewLine;  // *** DEBUG ***
                    //debugStr += " altimagepath[2]:" + sInfo.GetXmlProperty("genxml/lang/genxml/imagelist/genxml[2]/textbox/altimagepath") + Environment.NewLine;  // *** DEBUG ***
                    //debugStr += "-- Add AddSimplisityInfo END ---------------------------------------" + Environment.NewLine;  // *** DEBUG ***
                    simplisityData.AddSimplisityInfo(sInfo, sInfo.Lang);
                }

                //if (_debugmode)
                //{
                //    FileUtils.SaveFile(DNNrocketUtils.HomeDirectory() + "\\debug_SortListRecordsOnSave.xml", debugStr);
                //}
            }
            return simplisityData;
        }

        private Dictionary<int, SimplisityInfo> GetListInOrder(SimplisityInfo sInfo, string listName)
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
