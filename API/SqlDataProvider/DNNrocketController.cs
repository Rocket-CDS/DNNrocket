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

        #region "DNNrocket override DB Public Methods"

        /// <summary>
        /// override for Database Function
        /// </summary>
        /// <param name="itemId"></param>
        public override void Delete(int itemId, string tableName = "DNNrocket")
        {
            DataProvider.Instance().Delete(itemId, tableName);
        }

        /// <summary>
        /// override for Database Function
        /// </summary>
        public override void CleanData(string tableName = "DNNrocket")
        {
            DataProvider.Instance().CleanData(tableName);
        }

        /// <summary>
        /// override for Database Function.  Gets record, if lang is specified then lang xml in injected into the  base genxml node.
        /// </summary>
        /// <param name="itemId">itmeid of base genxml</param>
        /// <param name="lang">Culturecode of data to be injected into base genxml</param>
        /// <returns></returns>
        public override SimplisityInfo GetInfo(int itemId, string lang = "", string tableName = "DNNrocket")
        {
            return CBO.FillObject<SimplisityInfo>(DataProvider.Instance().GetInfo(itemId, lang, tableName));
        }

        public override SimplisityRecord GetRecord(int itemId, string tableName = "DNNrocket")
        {
            return CBO.FillObject<SimplisityRecord>(DataProvider.Instance().GetRecord(itemId, tableName));
        }

        public SimplisityRecord GetRecordLang(int parentitemId, string lang = "", bool debugMode = false, string tableName = "DNNrocket")
        {
            if (lang == "") lang = DNNrocketUtils.GetCurrentCulture();
            SimplisityRecord rtnInfo = null;
            rtnInfo = CBO.FillObject<SimplisityRecord>(DataProvider.Instance().GetRecordLang(parentitemId, lang, tableName));
            return rtnInfo;
        }

        public override int GetListCount(int portalId, int moduleId, string typeCode, string sqlSearchFilter = "", string lang = "", string tableName = "DNNrocket")
        {
            return DataProvider.Instance().GetListCount(portalId, moduleId, typeCode, sqlSearchFilter, lang, tableName);
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
        public override List<SimplisityInfo> GetList(int portalId, int moduleId, string typeCode, string sqlSearchFilter = "", string lang = "", string sqlOrderBy = "", int returnLimit = 0, int pageNumber = 0, int pageSize = 0, int recordCount = 0, string tableName = "DNNrocket")
        {
            return CBO.FillCollection<SimplisityInfo>(DataProvider.Instance().GetList(portalId, moduleId, typeCode, sqlSearchFilter, lang, sqlOrderBy, returnLimit, pageNumber, pageSize, recordCount, tableName));
        }

        /// <summary>
        /// Update record
        /// </summary>
        /// <param name="objInfo"></param>
        /// <param name="doindex"> if calling from the index function we don't want to index again.</param>
        /// <returns></returns>
        public int Update(SimplisityRecord objInfo, string tableName = "DNNrocket")
        {
            // save data
            objInfo.ModifiedDate = DateTime.Now;
            return DataProvider.Instance().Update(objInfo.ItemID, objInfo.PortalId, objInfo.ModuleId, objInfo.TypeCode, objInfo.XMLData, objInfo.GUIDKey, objInfo.ModifiedDate, objInfo.TextData, objInfo.XrefItemId, objInfo.ParentItemId, objInfo.UserId, objInfo.Lang, objInfo.SortOrder, tableName);
        }

        public void DeleteIndex(SimplisityRecord objInfo, string tableName = "DNNrocket")
        {
            var l = GetList(-1, -1, "", " and (R1.typecode = '" + objInfo.TypeCode + "LANGIDX' or R1.typecode like 'IDX_%')  and R1.parentitemid = " + objInfo.ItemID,"","",0,0,0,0, tableName);
            foreach (var i in l)
            {
                Delete(i.ItemID, tableName);
            }
        }

        /// <summary>
        /// rebuild language index merge record.
        /// </summary>
        /// <param name="objInfo"></param>
        /// <param name="itemid"></param>
        public void RebuildLangIndex(int portalId, int itemId, string tableName = "DNNrocket")
        {
            var objRec = GetRecord(itemId, tableName);
            if (objRec != null)
            {
                foreach (var l in DNNrocketUtils.GetCultureCodeList(portalId))
                {
                    var objRecLang = GetRecordLang(itemId, l, true, tableName);
                    if (objRecLang == null) objRecLang = new SimplisityInfo();
                    var info = new SimplisityInfo(objRec);
                    info.SetLangRecord(objRecLang);
                    var objIdx = GetRecordByGuidKey(-1, -1, info.TypeCode + "LANGIDX", itemId + "_" + l, "", tableName);
                    if (objIdx == null)
                    {
                        objIdx = new SimplisityRecord();
                        objIdx.ItemID = -1;
                        objIdx.PortalId = portalId;
                        objIdx.ModuleId = -1;
                        objIdx.XMLData = info.XMLData;
                        objIdx.TypeCode = info.TypeCode + "LANGIDX";
                        objIdx.GUIDKey = itemId + "_" + l;
                        objIdx.ParentItemId = itemId;
                        objIdx.Lang = l;
                    }
                    else
                    {
                        objIdx.XMLData = info.XMLData;
                    }
                    Update(objIdx, tableName);
                }
            }
        }

        /// <summary>
        /// Build Index Records
        /// </summary>
        /// <param name="objInfo"></param>
        /// <param name="itemid"></param>
        public void RebuildIndex(SimplisityRecord objInfo, string tableName = "DNNrocket")
        {
            if (!String.IsNullOrEmpty(objInfo.TypeCode) && objInfo.TypeCode.ToUpper() != "LANG") // do not process non-data records.
            {
                if (String.IsNullOrEmpty(objInfo.Lang)) // do not process language records 
                {
                    var dataitemid = objInfo.ItemID;
                    var entityTypeCode = objInfo.TypeCode;

                    var xrefitemid = objInfo.ParentItemId; // check for xref record, therefore parentitemid will be set.
                    if (xrefitemid <= 0)
                    {
                        xrefitemid = objInfo.ItemID; // not a xref record, so take the id.
                    }
                    var systemLinkRec = GetByGuidKey(-1, -1, "SYSTEMLINK", objInfo.TypeCode, "", tableName);
                    if (systemLinkRec == null)
                    {
                        // might be a xref, search for xref typecode
                        systemLinkRec = GetByGuidKey(-1, -1, "SYSTEMLINK", objInfo.GUIDKey, "", tableName);
                    }
                    if (systemLinkRec != null)
                    {
                        var xrefTypeCodeList = GetList(-1, -1, "SYSTEMLINK" + objInfo.TypeCode, " and R1.ParentitemId = '" + systemLinkRec.ItemID + "' ", "", "", 0, 0, 0, 0, tableName);
                        foreach (var i in xrefTypeCodeList)
                        {
                            var indexref = i.GUIDKey;
                            var xpath = i.GetXmlProperty("genxml/textbox/xpath");
                            if (xpath.StartsWith("genxml/lang/"))
                            {
                                // we need a langauge recrod.
                                var langList = DNNrocketUtils.GetCultureCodeList(objInfo.PortalId);
                                foreach (var lang in langList)
                                {
                                    var dataInfo = GetInfo(objInfo.ItemID, lang, tableName);
                                    if (dataInfo != null)
                                    {
                                        var value = dataInfo.GetXmlProperty(xpath);
                                        if (!String.IsNullOrEmpty(value))
                                        {
                                            CreateSystemLinkIdx(dataInfo.PortalId, indexref, xrefitemid, dataitemid, lang, value, tableName);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // non-langauge recrdo
                                var value = objInfo.GetXmlProperty(xpath);
                                if (!String.IsNullOrEmpty(value))
                                {
                                    CreateSystemLinkIdx(objInfo.PortalId, indexref, xrefitemid, dataitemid, "", value, tableName);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CreateSystemLinkIdx(int portalId, string indexref, int xrefitemid, int parentItemId, string lang, string value, string tableName = "DNNrocket")
        {
            // read is index exists already
            var strFilter = "and R1.ParentItemId = '" + parentItemId + "' and R1.Lang = '" + lang + "'";
            SimplisityInfo sRecord = null;
            var l = GetList(portalId, -1, "IDX_" + indexref, strFilter, "", "", 1,0,0,0, tableName);
            if (l.Count == 1)
            {
                sRecord = l[0];
            }
            if (sRecord == null)
            {
                sRecord = new SimplisityInfo();
                sRecord.ItemID = -1;
            }
            sRecord.PortalId = portalId;
            sRecord.ModuleId = -1;
            sRecord.ParentItemId = parentItemId;
            sRecord.TypeCode = "IDX_" + indexref;
            sRecord.ModifiedDate = DateTime.Now;
            sRecord.Lang = lang;
            sRecord.XrefItemId = xrefitemid;
            if (sRecord.GUIDKey != value)
            {
                sRecord.GUIDKey = value;
                DataProvider.Instance().Update(sRecord.ItemID, sRecord.PortalId, sRecord.ModuleId, sRecord.TypeCode, sRecord.XMLData, sRecord.GUIDKey, sRecord.ModifiedDate, sRecord.TextData, sRecord.XrefItemId, sRecord.ParentItemId, sRecord.UserId, sRecord.Lang, sRecord.SortOrder, tableName);
            }

        }

        /// <summary>
        /// Gte a single record from the Database using the EntityTypeCode.  This is usually used to fetch settings data "SETTINGS", where only 1 record will exist for the module.
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="systemId"></param>
        /// <param name="entityTypeCode"></param>
        /// <param name="selUserId"></param>
        /// <param name="entityTypeCodeLang"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public SimplisityInfo GetByType(int portalId, int moduleId, string entityTypeCode, string selUserId = "", string lang = "", string tableName = "DNNrocket")
        {
            var strFilter = "";
            if (selUserId != "")
            {
                strFilter += " and R1.UserId = " + selUserId + " ";
            }

            var l = CBO.FillCollection<SimplisityInfo>(DataProvider.Instance().GetList(portalId, moduleId, entityTypeCode, strFilter, lang, "", 1, 1, 1, 1, tableName));
            if (l.Count >= 1)
            {
                SimplisityInfo nbi = l[0];
                if (lang != "" && nbi.Lang != lang) return null; // GetByType will return invalid langauge if langaugue record does not exists, so test for it.
                return l[0];
            }
            return null;
        }
        public SimplisityRecord GetRecordByType(int portalId, int moduleId, string entityTypeCode, string selUserId = "", string lang = "", string tableName = "DNNrocket")
        {
            var strFilter = "";
            if (selUserId != "")
            {
                strFilter += " and R1.UserId = " + selUserId + " ";
            }

            var l = CBO.FillCollection<SimplisityInfo>(DataProvider.Instance().GetList(portalId, moduleId, entityTypeCode, strFilter, lang, "", 1, 1, 1, 1, tableName));
            if (l.Count >= 1)
            {
                SimplisityInfo nbi = l[0];
                var rtnRec = new SimplisityRecord(nbi);
                return rtnRec;
            }
            return null;
        }

        /// <summary>
        /// Get a single record back from the database, using the guyidkey (The seluserid is used to confirm the correct user.)
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="systemId  (using: moduleId)"></param>
        /// <param name="entityTypeCode"></param>
        /// <param name="guidKey"></param>
        /// <param name="selUserId"></param>
        /// <returns></returns>
        public SimplisityInfo GetByGuidKey(int portalId, int moduleId, string entityTypeCode, string guidKey, string selUserId = "", string tableName = "DNNrocket")
        {
            var strFilter = " and R1.GUIDKey = '" + guidKey + "' ";
            if (selUserId != "")
            {
                strFilter += " and R1.UserId = " + selUserId + " ";
            }

            var l = GetList(portalId, moduleId, entityTypeCode, strFilter,"", "", 1,0,0,0, tableName);
            if (l.Count == 0) return null;
            if (l.Count > 1)
            {
                for (int i = 1; i < l.Count; i++)
                {
                    // remove invalid DB entries
                    Delete(l[i].ItemID, tableName);
                }
            }
            return l[0];
        }

        public SimplisityRecord GetRecordByGuidKey(int portalId, int moduleId, string entityTypeCode, string guidKey, string selUserId = "", string tableName = "DNNrocket")
        {
            var strFilter = " and R1.GUIDKey = '" + guidKey + "' ";
            if (selUserId != "")
            {
                strFilter += " and R1.UserId = " + selUserId + " ";
            }

            var l = GetList(portalId, moduleId, entityTypeCode, strFilter, "", "", 1,0,0,0, tableName);
            if (l.Count == 0) return null;
            if (l.Count > 1)
            {
                for (int i = 1; i < l.Count; i++)
                {
                    // remove invalid DB entries
                    Delete(l[i].ItemID, tableName);
                }
            }
            if (l[0] == null) return null;
            var rtn = GetRecord(l[0].ItemID, tableName);
            return rtn;
        }


        public void FillEmptyLanguageFields(int baseParentItemId, String baseLang, string tableName = "DNNrocket")
        {
            var baseInfo = GetRecordLang(baseParentItemId, baseLang, true, tableName); // do NOT take cache
            if (baseInfo != null)
            {              
                foreach (var toLang in DNNrocketUtils.GetCultureCodeList(baseInfo.PortalId))
                {
                    if (toLang != baseInfo.Lang)
                    {
                        var updatedata = false;
                        var dlang = GetRecordLang(baseParentItemId, toLang, true, tableName); // do NOT take cache
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
                            Update(dlang, tableName);
                        }
                    }
                }
            }
        }

        public SimplisityInfo GetData(string GuidKey, string typeCode, string lang, int moduleId = -1, bool readOnly = false, string tableName = "DNNrocket")
        {
            SimplisityInfo nbi = null;
            var info = GetByGuidKey(PortalSettings.Current.PortalId, moduleId, typeCode, GuidKey,"", tableName);
            if (info == null && !readOnly)
            {
                // create record if not in DB
                info = new SimplisityInfo();
                info.GUIDKey = GuidKey;
                info.TypeCode = typeCode;
                info.ModuleId = moduleId;
                info.Lang = "";
                info.PortalId = PortalSettings.Current.PortalId;
                info.ItemID = Update(info, tableName);
            }

            if (info != null)
            {
                var nbilang = GetRecordLang(info.ItemID, lang,false, tableName);
                if (nbilang == null && !readOnly)
                {
                    //create in DB if lang is NOT in portal languages.
                    var inPortal = false;
                    foreach (var lg in DNNrocketUtils.GetCultureCodeList(PortalSettings.Current.PortalId))
                    {
                        if (lg == lang) inPortal = true;
                    }
                    if (!inPortal)
                    {
                        nbilang = new SimplisityInfo();
                        nbilang.GUIDKey = "";
                        nbilang.TypeCode = typeCode + "LANG";
                        nbilang.ParentItemId = info.ItemID;
                        nbilang.Lang = lang;
                        nbilang.ModuleId = moduleId;
                        nbilang.PortalId = PortalSettings.Current.PortalId;
                        nbilang.ItemID = Update(nbilang, tableName);
                    }

                    // create portal lang records if not in DB
                    foreach (var lg in DNNrocketUtils.GetCultureCodeList(PortalSettings.Current.PortalId))
                    {
                        nbilang = GetRecordLang(info.ItemID, lg, false, tableName);
                        if (nbilang == null)
                        {
                            nbilang = new SimplisityInfo();
                            nbilang.GUIDKey = "";
                            nbilang.TypeCode = typeCode + "LANG";
                            nbilang.ParentItemId = info.ItemID;
                            nbilang.Lang = lg;
                            nbilang.ModuleId = moduleId;
                            nbilang.PortalId = PortalSettings.Current.PortalId;
                            nbilang.ItemID = Update(nbilang, tableName);
                        }

                    }
                }
                nbi = GetInfo(info.ItemID, lang, tableName);
            }

            return nbi;
        }

        public SimplisityInfo SaveData(string GuidKey, string typeCode, SimplisityInfo sInfo, int moduleId = -1, string tableName = "DNNrocket")
        {
            var info = GetByGuidKey(PortalSettings.Current.PortalId, moduleId, typeCode, GuidKey,"", tableName);
            if (info == null)
            {
                // do read, so it creates the record and do a new read.
                info = GetData(GuidKey, typeCode, sInfo.Lang, moduleId ,false, tableName);
            }
            if (info != null)
            {
                info.PortalId = sInfo.PortalId;
                info.ModuleId = moduleId;
                info.TypeCode = typeCode;
                info.XMLData = sInfo.XMLData;
                info.GUIDKey = GuidKey;
                info.TextData = sInfo.TextData;
                info.ParentItemId = sInfo.ParentItemId;
                info.XrefItemId = sInfo.XrefItemId;
                info.Lang = "";
                info.UserId = sInfo.UserId;
                info.SortOrder = sInfo.SortOrder;

                info.RemoveLangRecord();
                var itemId = Update(info, tableName);
                var nbi2 = GetRecordLang(itemId, sInfo.Lang, false, tableName);
                if (nbi2 != null)
                {
                    nbi2.XMLData = sInfo.GetLangXml();
                    nbi2.TypeCode = info.TypeCode + "LANG";
                    nbi2.GUIDKey = "";
                    nbi2.ModuleId = moduleId;
                    nbi2.ParentItemId = itemId;
                    Update(nbi2, tableName);
                }

                RebuildIndex(sInfo, tableName);
                RebuildLangIndex(sInfo.PortalId, itemId, tableName);

                info = GetData(GuidKey, typeCode, sInfo.Lang, moduleId, false, tableName);
            }

            return info;
        }

        public SimplisityInfo SaveData(SimplisityInfo sInfo, string tableName = "DNNrocket")
        {
            var info = GetInfo(sInfo.ItemID, sInfo.Lang, tableName);
            if (info == null)
            {
                // do read, so it creates the record and do a new read.
                info = GetData(sInfo.TypeCode, sInfo.ItemID, sInfo.Lang, sInfo.ModuleId, false, tableName);
            }
            if (info != null)
            {
                info.PortalId = sInfo.PortalId;
                info.ModuleId = sInfo.ModuleId;
                info.TypeCode = sInfo.TypeCode;
                info.XMLData = sInfo.XMLData;
                info.GUIDKey = sInfo.GUIDKey;
                info.TextData = sInfo.TextData;
                info.ParentItemId = sInfo.ParentItemId;
                info.XrefItemId = sInfo.XrefItemId;
                info.Lang = "";
                info.UserId = sInfo.UserId;
                info.SortOrder = sInfo.SortOrder;

                info.RemoveLangRecord();
                var itemId = Update(info, tableName);

                var nbi2 = GetRecordLang(itemId, sInfo.Lang, false, tableName);
                if (nbi2 != null)
                {
                    nbi2.XMLData = sInfo.GetLangXml();
                    nbi2.TypeCode = info.TypeCode + "LANG";
                    nbi2.GUIDKey = "";
                    nbi2.ModuleId = sInfo.ModuleId;
                    nbi2.ParentItemId = itemId;
                    Update(nbi2, tableName);
                }

                RebuildIndex(sInfo, tableName);
                RebuildLangIndex(sInfo.PortalId, itemId, tableName);

                //CacheUtils.ClearAllCache(); // clear ALL cache.
                info = GetInfo(info.ItemID, sInfo.Lang, tableName);
            }

            return info;
        }


        public SimplisityInfo GetData(string typeCode, int ItemId, string lang, int moduleId = -1, bool readOnly = false, string tableName = "DNNrocket")
        {
            SimplisityInfo nbi = null;
            var info = GetInfo(ItemId, lang, tableName);
            if (info == null && !readOnly)
            {
                // create record if not in DB
                info = new SimplisityInfo();
                info.GUIDKey = "";
                info.TypeCode = typeCode;
                info.ModuleId = moduleId;
                info.PortalId = PortalSettings.Current.PortalId;
                info.ItemID = Update(info, tableName);
            }
            if (info != null)
            {
                var nbilang = GetRecordLang(info.ItemID, lang,false, tableName);
                if (nbilang == null)
                {
                    // create lang records if not in DB
                    foreach (var lg in DNNrocketUtils.GetCultureCodeList(PortalSettings.Current.PortalId))
                    {
                        nbilang = GetRecordLang(info.ItemID, lg, false, tableName);
                        if (nbilang == null)
                        {
                            nbilang = new SimplisityInfo();
                            nbilang.GUIDKey = "";
                            nbilang.TypeCode = typeCode + "LANG";
                            nbilang.ParentItemId = info.ItemID;
                            nbilang.Lang = lg;
                            info.ModuleId = moduleId;
                            nbilang.PortalId = PortalSettings.Current.PortalId;
                            nbilang.ItemID = Update(nbilang, tableName);
                        }
                    }
                }
                nbi = GetInfo(info.ItemID, lang, tableName);
            }
            return nbi;
        }


        #endregion

        #region "Get Save Record"

        public SimplisityRecord GetRecord(string GuidKey, string typeCode, int moduleId = -1, bool readOnly = false, string tableName = "DNNrocket")
        {
            //CacheUtils.ClearAllCache(); // clear ALL cache.
            var info = GetByGuidKey(PortalSettings.Current.PortalId, moduleId, typeCode, GuidKey,"", tableName);
            if (info == null && !readOnly)
            {
                // create record if not in DB
                info = new SimplisityInfo();
                info.GUIDKey = GuidKey;
                info.TypeCode = typeCode;
                info.ModuleId = moduleId;
                info.PortalId = PortalSettings.Current.PortalId;
                info.ItemID = Update(info, tableName);
            }
            return info;
        }

        public SimplisityRecord SaveRecord(string GuidKey, string typeCode, SimplisityRecord sRecord, int moduleId = -1, string tableName = "DNNrocket")
        {
            var info = GetRecordByGuidKey(PortalSettings.Current.PortalId, moduleId, typeCode, GuidKey,"", tableName);
            if (info == null)
            {
                // do read, so it creates the record and do a new read.
                info = GetRecord(GuidKey, typeCode, moduleId, false, tableName);
            }
            if (info != null)
            {
                info.PortalId = sRecord.PortalId;
                info.ModuleId = moduleId;
                info.TypeCode = typeCode;
                info.XMLData = sRecord.XMLData;
                info.GUIDKey = GuidKey;
                info.TextData = sRecord.TextData;
                info.ParentItemId = sRecord.ParentItemId;
                info.XrefItemId = sRecord.XrefItemId;
                info.Lang = "";
                info.UserId = sRecord.UserId;
                Update(info, tableName);
            }

            return info;
        }


        public SimplisityRecord GetRecord(string typeCode, int ItemId, int moduleId = -1, bool readOnly = false, string tableName = "DNNrocket")
        {
            var info = GetRecord(ItemId, tableName);
            if (info == null && !readOnly)
            {
                // create record if not in DB
                info = new SimplisityInfo();
                info.GUIDKey = "";
                info.TypeCode = typeCode;
                info.ModuleId = moduleId;
                info.PortalId = PortalSettings.Current.PortalId;
                info.ItemID = Update(info, tableName);
            }
            return GetRecord(info.ItemID, tableName);
        }

        public SimplisityRecord SaveRecord(SimplisityRecord sRecord, string tableName = "DNNrocket")
        {
            var info = GetRecord(sRecord.ItemID, tableName);
            if (info == null)
            {
                // do read, so it creates the record and do a new read.
                info = GetRecord(sRecord.TypeCode, sRecord.ItemID, sRecord.ModuleId,false, tableName);
            }
            if (info != null)
            {
                info.PortalId = sRecord.PortalId;
                info.ModuleId = sRecord.ModuleId;
                info.TypeCode = sRecord.TypeCode;
                info.XMLData = sRecord.XMLData;
                info.GUIDKey = sRecord.GUIDKey;
                info.TextData = sRecord.TextData;
                info.ParentItemId = sRecord.ParentItemId;
                info.XrefItemId = sRecord.XrefItemId;
                info.Lang = "";
                info.UserId = sRecord.UserId;
                Update(info, tableName);
                info = GetRecord(info.ItemID, tableName);
            }

            return info;
        }


        #endregion


    }

}
