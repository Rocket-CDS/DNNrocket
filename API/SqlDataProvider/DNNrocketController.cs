using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DNNrocketAPI.Interfaces;
using Simplisity;

namespace DNNrocketAPI
{

    public class DNNrocketController : DNNrocketCtrlInterface
    {

        #region "NBrightBuy override DB Public Methods"

        /// <summary>
        /// override for Database Function
        /// </summary>
        /// <param name="itemId"></param>
        public override void Delete(int itemId)
        {
            DataProvider.Instance().Delete(itemId);
        }

        /// <summary>
        /// override for Database Function
        /// </summary>
        public override void CleanData()
        {
            DataProvider.Instance().CleanData();
        }

        /// <summary>
        /// override for Database Function.  Gets record, if lang is specified then lang xml in injected into the  base genxml node.
        /// </summary>
        /// <param name="itemId">itmeid of base genxml</param>
        /// <param name="lang">Culturecode of data to be injected into base genxml</param>
        /// <returns></returns>
        public override SimplisityInfo GetInfo(int itemId, string lang = "")
        {
            return CBO.FillObject<SimplisityInfo>(DataProvider.Instance().GetInfo(itemId, lang));
        }

        public override SimplisityRecord GetRecord(int itemId)
        {
            return CBO.FillObject<SimplisityRecord>(DataProvider.Instance().GetRecord(itemId));
        }

        private SimplisityRecord GetRecordLang(int parentitemId, string lang = "", bool debugMode = false)
        {
            if (lang == "") lang = DNNrocketUtils.GetCurrentCulture();
            SimplisityRecord rtnInfo = null;
            rtnInfo = CBO.FillObject<SimplisityRecord>(DataProvider.Instance().GetRecordLang(parentitemId, lang));
            return rtnInfo;
        }

        public override int GetListCount(int portalId, int moduleId, string typeCode, string sqlSearchFilter = "", string lang = "")
        {
            return DataProvider.Instance().GetListCount(portalId, moduleId, typeCode, sqlSearchFilter, lang);
        }


        /// <summary>
        /// override for Database Function
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="moduleId"></param>
        /// <param name="typeCode"></param>
        /// <param name="sqlSearchFilter"></param>
        /// <param name="sqlOrderBy"></param>
        /// <param name="returnLimit"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <param name="typeCodeLang"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public override List<SimplisityInfo> GetList(int portalId, int moduleId, string typeCode, string sqlSearchFilter = "", string lang = "", string sqlOrderBy = "", int returnLimit = 0, int pageNumber = 0, int pageSize = 0, int recordCount = 0)
        {
            return CBO.FillCollection<SimplisityInfo>(DataProvider.Instance().GetList(portalId, moduleId, typeCode, sqlSearchFilter, lang, sqlOrderBy, returnLimit, pageNumber, pageSize, recordCount));
        }

        /// <summary>
        /// override for Database Function
        /// </summary>
        /// <param name="objInfo"></param>
        /// <returns></returns>
        public override int Update(SimplisityRecord objInfo)
        {
            // save data
            objInfo.ModifiedDate = DateTime.Now;
            var itemid = DataProvider.Instance().Update(objInfo.ItemID, objInfo.PortalId, objInfo.ModuleId, objInfo.TypeCode, objInfo.XMLData, objInfo.GUIDKey, objInfo.ModifiedDate, objInfo.TextData, objInfo.XrefItemId, objInfo.ParentItemId, objInfo.UserId, objInfo.Lang);

            RebuildIndex(objInfo, itemid);

            return itemid;
        }

        public void DeleteIndex(SimplisityRecord objInfo)
        {
            var l = GetList(-1, -1, "", " and (R1.typecode = '" + objInfo.TypeCode + "IDX' or R1.typecode = '" + objInfo.TypeCode + "LANGIDX' or R1.typecode like '" + objInfo.TypeCode + "IDX_%')  and R1.parentitemid = " + objInfo.ItemID);
            foreach (var i in l)
            {
                Delete(i.ItemID);
            }
        }

        public void RebuildIndex(SimplisityRecord objInfo, int itemid)
        {
            //-------------------------------------------------------------------
            // Save Merged Lang data.
            //-------------------------------------------------------------------
            if (String.IsNullOrEmpty(objInfo.Lang))
            {
                var langList = DNNrocketUtils.GetCultureCodeList(objInfo.PortalId);
                foreach (var l in langList)
                {
                    var idxLang = GetByGuidKey(objInfo.PortalId, -1, objInfo.TypeCode + "LANGIDX", objInfo.ItemID.ToString() + "_" + l);
                    if (idxLang != null)
                    {
                        var langXml = idxLang.GetLangXml();
                        idxLang.XMLData = objInfo.XMLData;
                        idxLang.SetLangXml(langXml);
                        DataProvider.Instance().Update(idxLang.ItemID, idxLang.PortalId, idxLang.ModuleId, idxLang.TypeCode, idxLang.XMLData, idxLang.GUIDKey, idxLang.ModifiedDate, idxLang.TextData, idxLang.XrefItemId, idxLang.ParentItemId, idxLang.UserId, idxLang.Lang);
                    }
                }
            }
            else
            {
                var idxLang = GetByGuidKey(objInfo.PortalId, -1, objInfo.TypeCode + "IDX", objInfo.ParentItemId.ToString() + "_" + objInfo.Lang);
                if (idxLang != null)
                {
                    idxLang.RemoveLangRecord();
                    idxLang.SetLangXml(objInfo.XMLData);
                    DataProvider.Instance().Update(idxLang.ItemID, idxLang.PortalId, idxLang.ModuleId, idxLang.TypeCode, idxLang.XMLData, idxLang.GUIDKey, idxLang.ModifiedDate, idxLang.TextData, idxLang.XrefItemId, idxLang.ParentItemId, idxLang.UserId, idxLang.Lang);
                }
                else
                {
                    // Langauge record update.
                    var baseRecord = GetRecord(objInfo.ParentItemId);
                    if (baseRecord != null)
                    {
                        var sRecord = new SimplisityInfo();
                        sRecord.PortalId = baseRecord.PortalId;
                        sRecord.ModuleId = -1;
                        sRecord.ParentItemId = baseRecord.ItemID;
                        sRecord.TypeCode = objInfo.TypeCode + "IDX";
                        sRecord.ModifiedDate = DateTime.Now;
                        sRecord.XMLData = baseRecord.XMLData;
                        sRecord.Lang = objInfo.Lang;
                        sRecord.GUIDKey = objInfo.ParentItemId.ToString() + "_" + objInfo.Lang;
                        sRecord.SetLangXml(objInfo.XMLData);
                        DataProvider.Instance().Update(sRecord.ItemID, sRecord.PortalId, sRecord.ModuleId, sRecord.TypeCode, sRecord.XMLData, sRecord.GUIDKey, sRecord.ModifiedDate, sRecord.TextData, sRecord.XrefItemId, sRecord.ParentItemId, sRecord.UserId, sRecord.Lang);
                    }

                }
            }

            //-------------------------------------------------------------------
            // build index records.
            //-------------------------------------------------------------------

            var dataLang = "";
            var dataitemid = itemid;
            var entityTypeCode = objInfo.TypeCode;
            if (objInfo.Lang != "" && entityTypeCode.EndsWith("LANG"))
            {
                // langauge record, alter as required.
                entityTypeCode = entityTypeCode.Substring(0, entityTypeCode.Length - 4);
                dataLang = objInfo.Lang;
                var langInfo = GetRecord(dataitemid);
                dataitemid = langInfo.ParentItemId;
            }
            var systemLink = GetByGuidKey(-1, -1, "SYSTEMLINK", entityTypeCode);
            if (systemLink != null)
            {
                var systemInfo = GetInfo(systemLink.ParentItemId);
                if (systemLink != null)
                {
                    var idxfieldlist = systemInfo.GetList("idxfielddata");
                    foreach (var i in idxfieldlist)
                    {
                        if (entityTypeCode == i.GetXmlProperty("genxml/dropdownlist/entitytypecode"))
                        {
                            var xpath = i.GetXmlProperty("genxml/textbox/xpath");
                            if (xpath.StartsWith("genxml/lang/") && objInfo.Lang != "")
                            {
                                xpath = xpath.Substring(("genxml/lang/").Length);
                            }
                            var value = objInfo.GetXmlProperty(xpath);
                            if (value != "")
                            {
                                var indexref = i.GetXmlProperty("genxml/textbox/indexref");
                                // read is index exists already
                                var strFilter = "and R1.ParentItemId = '" + dataitemid + "' and R1.Lang = '" + dataLang + "'";
                                SimplisityInfo sRecord = null;
                                var l = GetList(objInfo.PortalId, objInfo.ModuleId, entityTypeCode + "IDX_" + indexref, strFilter, "", "", 1);
                                if (l.Count == 1)
                                {
                                    sRecord = l[0];
                                }
                                if (sRecord == null)
                                {
                                    sRecord = new SimplisityInfo();
                                    sRecord.ItemID = -1;
                                }
                                sRecord.PortalId = objInfo.PortalId;
                                sRecord.ModuleId = -1;
                                sRecord.ParentItemId = dataitemid;
                                sRecord.TypeCode = entityTypeCode + "IDX_" + indexref;
                                sRecord.ModifiedDate = DateTime.Now;
                                sRecord.Lang = dataLang;
                                if (sRecord.GUIDKey != value)
                                {
                                    sRecord.GUIDKey = value;
                                    DataProvider.Instance().Update(sRecord.ItemID, sRecord.PortalId, sRecord.ModuleId, sRecord.TypeCode, sRecord.XMLData, sRecord.GUIDKey, sRecord.ModifiedDate, sRecord.TextData, sRecord.XrefItemId, sRecord.ParentItemId, sRecord.UserId, sRecord.Lang);
                                }
                            }
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Gte a single record from the Database using the EntityTypeCode.  This is usually used to fetch settings data "SETTINGS", where only 1 record will exist for the module.
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="moduleId"></param>
        /// <param name="entityTypeCode"></param>
        /// <param name="selUserId"></param>
        /// <param name="entityTypeCodeLang"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public SimplisityInfo GetByType(int portalId, int moduleId, string entityTypeCode, string selUserId = "", string lang = "")
        {
            var strFilter = "";
            if (selUserId != "")
            {
                strFilter += " and R1.UserId = " + selUserId + " ";
            }

            var l = CBO.FillCollection<SimplisityInfo>(DataProvider.Instance().GetList(portalId, moduleId, entityTypeCode, strFilter, lang, "", 1, 1, 1, 1));
            if (l.Count >= 1)
            {
                SimplisityInfo nbi = l[0];
                if (lang != "" && nbi.Lang != lang) return null; // GetByType will return invalid langauge if langaugue record does not exists, so test for it.
                return l[0];
            }
            return null;
        }

        /// <summary>
        /// Get a single record back from the database, using the guyidkey (The seluserid is used to confirm the correct user.)
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="moduleId"></param>
        /// <param name="entityTypeCode"></param>
        /// <param name="guidKey"></param>
        /// <param name="selUserId"></param>
        /// <returns></returns>
        public SimplisityInfo GetByGuidKey(int portalId, int moduleId, string entityTypeCode, string guidKey, string selUserId = "")
        {
            var strFilter = " and R1.GUIDKey = '" + guidKey + "' ";
            if (selUserId != "")
            {
                strFilter += " and R1.UserId = " + selUserId + " ";
            }

            var l = GetList(portalId, moduleId, entityTypeCode, strFilter,"", "", 1);
            if (l.Count == 0) return null;
            if (l.Count > 1)
            {
                for (int i = 1; i < l.Count; i++)
                {
                    // remove invalid DB entries
                    Delete(l[i].ItemID);
                }
            }
            return l[0];
        }


        public void FillEmptyLanguageFields(int baseParentItemId, String baseLang)
        {
            var baseInfo = GetRecordLang(baseParentItemId, baseLang, true); // do NOT take cache
            if (baseInfo != null)
            {              
                foreach (var toLang in DNNrocketUtils.GetCultureCodeList(baseInfo.PortalId))
                {
                    if (toLang != baseInfo.Lang)
                    {
                        var updatedata = false;
                        var dlang = GetRecordLang(baseParentItemId, toLang, true); // do NOT take cache
                        if (dlang != null)
                        {
                            var nodList = baseInfo.XMLDoc.SelectNodes("genxml/textbox/*");
                            if (nodList != null)
                            {
                                foreach (XmlNode nod in nodList)
                                {
                                    if (nod.InnerText.Trim() != "")
                                    {
                                        if (dlang.GetXmlProperty("genxml/textbox/" + nod.Name) == "")
                                        {
                                            dlang.SetXmlProperty("genxml/textbox/" + nod.Name, nod.InnerText);
                                            updatedata = true;
                                        }
                                    }
                                }
                            }

                            var nodList2i = baseInfo.XMLDoc.SelectNodes("genxml/imgs/genxml");
                            if (nodList2i != null)
                            {
                                for (int i = 1; i <= nodList2i.Count; i++)
                                {
                                    var nodList2 = baseInfo.XMLDoc.SelectNodes("genxml/imgs/genxml[" + i + "]/textbox/*");
                                    if (nodList2 != null)
                                    {
                                        foreach (XmlNode nod in nodList2)
                                        {
                                            if (nod.InnerText.Trim() != "")
                                            {
                                                if (dlang.GetXmlProperty("genxml/imgs/genxml[" + i + "]/textbox/" + nod.Name) == "")
                                                {
                                                    if (dlang.XMLDoc.SelectSingleNode("genxml/imgs/genxml[" + i + "]") == null)
                                                    {
                                                        var baseXml = baseInfo.XMLDoc.SelectSingleNode("genxml/imgs/genxml[" + i + "]");
                                                        if (baseXml != null)
                                                        {
                                                            if (dlang.XMLDoc.SelectSingleNode("genxml/imgs") == null)
                                                            {
                                                                dlang.SetXmlProperty("genxml/imags","");
                                                            }
                                                            dlang.AddXmlNode(baseXml.OuterXml, "genxml", "genxml/imgs");
                                                        }
                                                    }
                                                    dlang.SetXmlProperty("genxml/imgs/genxml[" + i + "]/textbox/" + nod.Name, nod.InnerText);
                                                    updatedata = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }


                            var nodList3i = baseInfo.XMLDoc.SelectNodes("genxml/docs/genxml");
                            if (nodList3i != null)
                            {
                                for (int i = 1; i <= nodList3i.Count; i++)
                                {
                                    var nodList3 = baseInfo.XMLDoc.SelectNodes("genxml/docs/genxml[" + i + "]/textbox/*");
                                    if (nodList3 != null)
                                    {
                                        foreach (XmlNode nod in nodList3)
                                        {
                                            if (nod.InnerText.Trim() != "")
                                            {
                                                if (dlang.GetXmlProperty("genxml/docs/genxml[" + i + "]/textbox/" + nod.Name) == "")
                                                {
                                                    if (dlang.XMLDoc.SelectSingleNode("genxml/docs/genxml[" + i + "]") == null)
                                                    {
                                                        var baseXml = baseInfo.XMLDoc.SelectSingleNode("genxml/docs/genxml[" + i + "]");
                                                        if (baseXml != null)
                                                        {
                                                            if (dlang.XMLDoc.SelectSingleNode("genxml/docs") == null)
                                                            {
                                                                dlang.SetXmlProperty("genxml/docs", "");
                                                            }
                                                            dlang.AddXmlNode(baseXml.OuterXml, "genxml", "genxml/docs");
                                                        }
                                                    }
                                                    dlang.SetXmlProperty("genxml/docs/genxml[" + i + "]/textbox/" + nod.Name, nod.InnerText);
                                                    updatedata = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (updatedata)
                        {
                            Update(dlang);
                        }
                    }
                }
            }
        }

        public SimplisityInfo GetData(string GuidKey, string typeCode, string lang)
        {
            //CacheUtils.ClearAllCache(); // clear ALL cache.
            var info = GetByGuidKey(PortalSettings.Current.PortalId, -1, typeCode, GuidKey);
            if (info == null)
            {
                // create record if not in DB
                info = new SimplisityInfo();
                info.GUIDKey = GuidKey;
                info.TypeCode = typeCode;
                info.ModuleId = -1;
                info.PortalId = PortalSettings.Current.PortalId;
                info.ItemID = Update(info);
            }
            var nbilang = GetRecordLang(info.ItemID, lang);
            if (nbilang == null)
            {
                // create lang records if not in DB
                foreach (var lg in DNNrocketUtils.GetCultureCodeList(PortalSettings.Current.PortalId))
                {
                    nbilang = GetRecordLang(info.ItemID, lg);
                    if (nbilang == null)
                    {
                        nbilang = new SimplisityInfo();
                        nbilang.GUIDKey = "";
                        nbilang.TypeCode = typeCode + "LANG";
                        nbilang.ParentItemId = info.ItemID;
                        nbilang.Lang = lg;
                        nbilang.ModuleId = -1;
                        nbilang.PortalId = PortalSettings.Current.PortalId;
                        nbilang.ItemID = Update(nbilang);
                    }
                }
            }

            // do edit field data if a itemid has been selected
            var nbi = GetInfo(info.ItemID, lang);
            return nbi;
        }

        public SimplisityInfo SaveData(string GuidKey, string typeCode, SimplisityInfo postInfo)
        {
            var info = GetByGuidKey(PortalSettings.Current.PortalId, -1, typeCode, GuidKey);
            if (info == null)
            {
                // do read, so it creates the record and do a new read.
                info = GetData(GuidKey, typeCode, postInfo.Lang);
            }
            if (info != null)
            {
                var smiLang = postInfo.GetLangRecord();
                smiLang.Lang = postInfo.Lang;
                info.XMLData = postInfo.XMLData;
                info.RemoveLangRecord();
                info.Lang = "";
                Update(info);
                var nbi2 = GetRecordLang(info.ItemID, smiLang.Lang);
                if (nbi2 == null)
                {
                    smiLang.ItemID = -1; // add if null (should not happen)
                }
                else
                {
                    smiLang.ItemID = nbi2.ItemID;
                }
                smiLang.ParentItemId = info.ItemID;
                smiLang.TypeCode = info.TypeCode + "LANG";
                smiLang.GUIDKey = "";
                Update(smiLang);
                CacheUtils.ClearAllCache(); // clear ALL cache.
                info = GetByGuidKey(PortalSettings.Current.PortalId, -1, typeCode, GuidKey);
            }

            return info;
        }

        public SimplisityInfo GetData(string typeCode, int ItemId, string lang)
        {
            var info = GetInfo(ItemId, lang);
            if (info == null)
            {
                // create record if not in DB
                info = new SimplisityInfo();
                info.GUIDKey = "";
                info.TypeCode = typeCode;
                info.ModuleId = -1;
                info.PortalId = PortalSettings.Current.PortalId;
                info.ItemID = Update(info);
            }
            var nbilang = GetRecordLang(info.ItemID, lang);
            if (nbilang == null)
            {
                // create lang records if not in DB
                foreach (var lg in DNNrocketUtils.GetCultureCodeList(PortalSettings.Current.PortalId))
                {
                    nbilang = GetRecordLang(info.ItemID, lg);
                    if (nbilang == null)
                    {
                        nbilang = new SimplisityInfo();
                        nbilang.GUIDKey = "";
                        nbilang.TypeCode = typeCode + "LANG";
                        nbilang.ParentItemId = info.ItemID;
                        nbilang.Lang = lg;
                        nbilang.ModuleId = -1;
                        nbilang.PortalId = PortalSettings.Current.PortalId;
                        nbilang.ItemID = Update(nbilang);
                    }
                }
            }

            // do edit field data if a itemid has been selected
            var nbi = GetInfo(info.ItemID, lang);
            return nbi;
        }

        public SimplisityInfo SaveData(SimplisityInfo sInfo)
        {
            var info = GetInfo(sInfo.ItemID, sInfo.Lang);
            if (info == null)
            {
                // do read, so it creates the record and do a new read.
                info = GetData(sInfo.TypeCode, sInfo.ItemID, sInfo.Lang);
            }
            if (info != null)
            {
                var smiLang = sInfo.GetLangRecord();
                smiLang.Lang = sInfo.Lang;
                info.XMLData = sInfo.XMLData;
                info.RemoveLangRecord();
                info.Lang = "";
                Update(info);
                var nbi2 = GetRecordLang(info.ItemID, smiLang.Lang);
                if (nbi2 == null)
                {
                    smiLang.ItemID = -1; // add if null (should not happen)
                }
                else
                {
                    smiLang.ItemID = nbi2.ItemID;
                }
                smiLang.ParentItemId = info.ItemID;
                smiLang.TypeCode = info.TypeCode + "LANG";
                smiLang.GUIDKey = "";
                Update(smiLang);
                CacheUtils.ClearAllCache(); // clear ALL cache.
                info = GetInfo(info.ItemID, smiLang.Lang);
            }

            return info;
        }


        #endregion


    }

}
