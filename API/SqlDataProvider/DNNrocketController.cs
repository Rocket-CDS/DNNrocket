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

        public override int GetListCount(int portalId, int moduleId, string typeCode, string sqlSearchFilter = "", string lang = "", int systemId = -1)
        {
            return DataProvider.Instance().GetListCount(portalId, moduleId, typeCode, sqlSearchFilter, lang, systemId);
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
        public override List<SimplisityInfo> GetList(int portalId, int moduleId, string typeCode, string sqlSearchFilter = "", string lang = "", string sqlOrderBy = "", int returnLimit = 0, int pageNumber = 0, int pageSize = 0, int recordCount = 0,int systemId = -1)
        {
            return CBO.FillCollection<SimplisityInfo>(DataProvider.Instance().GetList(portalId, moduleId, typeCode, sqlSearchFilter, lang, sqlOrderBy, returnLimit, pageNumber, pageSize, recordCount, systemId));
        }

        /// <summary>
        /// Update record
        /// </summary>
        /// <param name="objInfo"></param>
        /// <param name="doindex"> if calling from the index function we don't want to index again.</param>
        /// <returns></returns>
        public override int Update(SimplisityRecord objInfo, bool doindex = true)
        {
            var itemid = -1;
            if (!String.IsNullOrEmpty(objInfo.TypeCode) && objInfo.TypeCode.ToUpper() != "LANG") // do not process non-data records.
            {
                // save data
                objInfo.ModifiedDate = DateTime.Now;
                itemid = DataProvider.Instance().Update(objInfo.ItemID, objInfo.PortalId, objInfo.ModuleId, objInfo.TypeCode, objInfo.XMLData, objInfo.GUIDKey, objInfo.ModifiedDate, objInfo.TextData, objInfo.XrefItemId, objInfo.ParentItemId, objInfo.UserId, objInfo.Lang, objInfo.SystemId);

                if (doindex)
                {
                    if (objInfo.Lang == "")
                    {
                        RebuildIndex(objInfo);
                    }
                    else
                    {
                        RebuildLangIndex(objInfo, itemid);
                    }
                }
            }
            return itemid;
        }

        public void DeleteIndex(SimplisityRecord objInfo)
        {
            var l = GetList(-1, -1, "", " and (R1.typecode = '" + objInfo.TypeCode + "LANGIDX' or R1.typecode like 'IDX_%')  and R1.parentitemid = " + objInfo.ItemID);
            foreach (var i in l)
            {
                Delete(i.ItemID);
            }
        }

        /// <summary>
        /// rebuild language index merge record.
        /// </summary>
        /// <param name="objInfo"></param>
        /// <param name="itemid"></param>
        public void RebuildLangIndex(SimplisityRecord objInfo, int itemid)
        {
            if (!String.IsNullOrEmpty(objInfo.TypeCode) && objInfo.TypeCode.ToUpper() != "LANG") // do not process non-data records.
            {
                if (!String.IsNullOrEmpty(objInfo.Lang))
                {
                    // do langauge record.
                    var baseRecord = GetRecord(objInfo.ParentItemId);
                    var saveItemId = 0;
                    var idxLang = GetByGuidKey(objInfo.PortalId, -1, objInfo.TypeCode + "IDX", objInfo.ParentItemId.ToString() + "_" + objInfo.Lang);
                    if (idxLang != null)
                    {
                        idxLang.XMLData = baseRecord.XMLData;
                        idxLang.SetLangXml(objInfo.XMLData);
                        saveItemId = DataProvider.Instance().Update(idxLang.ItemID, idxLang.PortalId, idxLang.ModuleId, idxLang.TypeCode, idxLang.XMLData, idxLang.GUIDKey, idxLang.ModifiedDate, idxLang.TextData, idxLang.XrefItemId, idxLang.ParentItemId, idxLang.UserId, idxLang.Lang, idxLang.SystemId);
                    }
                    else
                    {
                        // Language record update.
                        if (baseRecord != null && !String.IsNullOrEmpty(baseRecord.TypeCode))
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
                            saveItemId = DataProvider.Instance().Update(sRecord.ItemID, sRecord.PortalId, sRecord.ModuleId, sRecord.TypeCode, sRecord.XMLData, sRecord.GUIDKey, sRecord.ModifiedDate, sRecord.TextData, sRecord.XrefItemId, sRecord.ParentItemId, sRecord.UserId, sRecord.Lang, sRecord.SystemId);
                        }

                    }
                    // we can only index after the langauge update, so we have all data (language data) in the DB
                    // This will call multiple times, once for each language.
                    if (saveItemId > 0)
                    {
                        var langInfo = GetRecord(saveItemId);
                        var baseInfo = GetRecord(langInfo.ParentItemId);
                        RebuildIndex(baseInfo);
                    }
                }
                else
                {
                    // we have a base language, so we need to update all language LANGIDX records.
                    var l = GetList(-1, -1, "", " and (R1.typecode = '" + objInfo.TypeCode + "LANGIDX')  and R1.parentitemid = " + objInfo.ItemID);
                    foreach (var i in l)
                    {
                        var langxml = i.GetLangXml();
                        i.XMLData = objInfo.XMLData;
                        i.SetLangXml(langxml);
                        Update(i, false);
                    }
                }
            }
        }

        /// <summary>
        /// Build Index Records
        /// </summary>
        /// <param name="objInfo"></param>
        /// <param name="itemid"></param>
        public void RebuildIndex(SimplisityRecord objInfo)
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

                    var systemId = objInfo.SystemId;
                    var systemInfo = GetRecord(systemId);

                    if (systemInfo != null)
                    {
                        var systemLinkRec = GetByGuidKey(-1, -1, "SYSTEMLINK", objInfo.TypeCode);
                        if (systemLinkRec == null)
                        {
                            // might be a xref, search for xref typecode
                            systemLinkRec = GetByGuidKey(-1, -1, "SYSTEMLINK", objInfo.GUIDKey);
                        }
                        if (systemLinkRec != null)
                        {
                            var xrefTypeCodeList = GetList(-1, -1, "SYSTEMLINK" + objInfo.TypeCode, " and R1.ParentitemId = '" + systemLinkRec.ItemID + "' ");
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
                                        var dataInfo = GetInfo(objInfo.ItemID, lang);
                                        if (dataInfo != null)
                                        {
                                            var value = dataInfo.GetXmlProperty(xpath);
                                            if (!String.IsNullOrEmpty(value))
                                            {
                                                CreateSystemLinkIdx(dataInfo.PortalId, dataInfo.SystemId, indexref, xrefitemid, dataitemid, lang, value);
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
                                        CreateSystemLinkIdx(objInfo.PortalId, objInfo.SystemId, indexref, xrefitemid, dataitemid, "", value);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CreateSystemLinkIdx(int portalId, int systemId, string indexref, int xrefitemid, int parentItemId, string lang, string value)
        {
            // read is index exists already
            var strFilter = "and R1.ParentItemId = '" + parentItemId + "' and R1.Lang = '" + lang + "'";
            SimplisityInfo sRecord = null;
            var l = GetList(portalId, -1, "IDX_" + indexref, strFilter, "", "", 1,0,0,0, systemId);
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
            sRecord.SystemId = systemId;
            if (sRecord.GUIDKey != value)
            {
                sRecord.GUIDKey = value;
                DataProvider.Instance().Update(sRecord.ItemID, sRecord.PortalId, sRecord.ModuleId, sRecord.TypeCode, sRecord.XMLData, sRecord.GUIDKey, sRecord.ModifiedDate, sRecord.TextData, sRecord.XrefItemId, sRecord.ParentItemId, sRecord.UserId, sRecord.Lang, sRecord.SystemId);
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
        /// <param name="systemId  (using: moduleId)"></param>
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

        public SimplisityRecord GetRecordByGuidKey(int portalId, int moduleId, string entityTypeCode, string guidKey, string selUserId = "")
        {
            var strFilter = " and R1.GUIDKey = '" + guidKey + "' ";
            if (selUserId != "")
            {
                strFilter += " and R1.UserId = " + selUserId + " ";
            }

            var l = GetList(portalId, moduleId, entityTypeCode, strFilter, "", "", 1);
            if (l.Count == 0) return null;
            if (l.Count > 1)
            {
                for (int i = 1; i < l.Count; i++)
                {
                    // remove invalid DB entries
                    Delete(l[i].ItemID);
                }
            }
            if (l[0] == null) return null;
            var rtn = GetRecord(l[0].ItemID);
            return rtn;
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

        public SimplisityInfo GetData(string GuidKey, string typeCode, string lang, int systemId = -1, int moduleId = -1, bool readOnly = false)
        {
            SimplisityInfo nbi = null;
            //CacheUtils.ClearAllCache(); // clear ALL cache.
            var info = GetByGuidKey(PortalSettings.Current.PortalId, moduleId, typeCode, GuidKey);
            if (info == null && !readOnly)
            {
                // create record if not in DB
                info = new SimplisityInfo();
                info.GUIDKey = GuidKey;
                info.TypeCode = typeCode;
                info.SystemId = systemId;
                info.ModuleId = moduleId;                
                info.PortalId = PortalSettings.Current.PortalId;
                info.ItemID = Update(info);
            }

            if (info != null)
            {
                var nbilang = GetRecordLang(info.ItemID, lang);
                if (nbilang == null)
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
                        nbilang.SystemId = systemId;
                        nbilang.ModuleId = moduleId;
                        nbilang.PortalId = PortalSettings.Current.PortalId;
                        nbilang.ItemID = Update(nbilang);
                    }

                    // create portal lang records if not in DB
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
                            nbilang.SystemId = systemId;
                            nbilang.ModuleId = moduleId;
                            nbilang.PortalId = PortalSettings.Current.PortalId;
                            nbilang.ItemID = Update(nbilang);
                        }
                    }
                }
                nbi = GetInfo(info.ItemID, lang);
            }

            return nbi;
        }

        public SimplisityInfo SaveData(string GuidKey, string typeCode, SimplisityInfo sInfo, int systemId = -1, int moduleId = -1)
        {
            var info = GetByGuidKey(PortalSettings.Current.PortalId, moduleId, typeCode, GuidKey);
            if (info == null)
            {
                // do read, so it creates the record and do a new read.
                info = GetData(GuidKey, typeCode, sInfo.Lang, systemId, moduleId);
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
                info.SystemId = systemId;

                info.RemoveLangRecord();
                var itemId = Update(info);
                var nbi2 = GetRecordLang(itemId, sInfo.Lang);
                if (nbi2 != null)
                {
                    nbi2.XMLData = sInfo.GetLangXml();
                    nbi2.TypeCode = info.TypeCode + "LANG";
                    nbi2.GUIDKey = "";
                    nbi2.SystemId = systemId;
                    nbi2.ModuleId = moduleId;
                    nbi2.ParentItemId = itemId;
                    Update(nbi2);
                }
                //CacheUtils.ClearAllCache(); // clear ALL cache.
                info = GetData(GuidKey, typeCode, sInfo.Lang, systemId, moduleId);
            }

            return info;
        }

        public SimplisityInfo GetData(string typeCode, int ItemId, string lang, int systemId = -1, int moduleId = -1, bool readOnly = false)
        {
            SimplisityInfo nbi = null;
            var info = GetInfo(ItemId, lang);
            if (info == null && !readOnly)
            {
                // create record if not in DB
                info = new SimplisityInfo();
                info.GUIDKey = "";
                info.TypeCode = typeCode;
                info.SystemId = systemId;
                info.ModuleId = moduleId;
                info.PortalId = PortalSettings.Current.PortalId;
                info.ItemID = Update(info);
            }
            if (info != null)
            {
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
                            info.SystemId = systemId;
                            info.ModuleId = moduleId;
                            nbilang.PortalId = PortalSettings.Current.PortalId;
                            nbilang.ItemID = Update(nbilang);
                        }
                    }
                }
                nbi = GetInfo(info.ItemID, lang);
            }
            return nbi;
        }

        public SimplisityInfo SaveData(SimplisityInfo sInfo, int systemId)
        {
            var info = GetInfo(sInfo.ItemID, sInfo.Lang);
            if (info == null)
            {
                // do read, so it creates the record and do a new read.
                info = GetData(sInfo.TypeCode, sInfo.ItemID, sInfo.Lang, systemId, sInfo.ModuleId);
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
                info.SystemId = systemId;

                info.RemoveLangRecord();
                var itemId = Update(info);

                var nbi2 = GetRecordLang(itemId, sInfo.Lang);
                if (nbi2 != null)
                {
                    nbi2.XMLData = sInfo.GetLangXml();
                    nbi2.TypeCode = info.TypeCode + "LANG";
                    nbi2.GUIDKey = "";
                    nbi2.SystemId = systemId;
                    nbi2.ModuleId = sInfo.ModuleId;
                    nbi2.ParentItemId = itemId;
                    Update(nbi2);
                }

                //CacheUtils.ClearAllCache(); // clear ALL cache.
                info = GetInfo(info.ItemID, sInfo.Lang);
            }

            return info;
        }

        #endregion

        #region "Get Save Record"

        public SimplisityRecord GetRecord(string GuidKey, string typeCode, int systemId = -1, int moduleId = -1, bool readOnly = false)
        {
            //CacheUtils.ClearAllCache(); // clear ALL cache.
            var info = GetByGuidKey(PortalSettings.Current.PortalId, moduleId, typeCode, GuidKey);
            if (info == null && !readOnly)
            {
                // create record if not in DB
                info = new SimplisityInfo();
                info.GUIDKey = GuidKey;
                info.TypeCode = typeCode;
                info.SystemId = systemId;
                info.ModuleId = moduleId;
                info.PortalId = PortalSettings.Current.PortalId;
                info.ItemID = Update(info);
            }
            return info;
        }

        public SimplisityRecord SaveRecord(string GuidKey, string typeCode, SimplisityRecord sRecord, int systemId = -1, int moduleId = -1)
        {
            var info = GetRecordByGuidKey(PortalSettings.Current.PortalId, moduleId, typeCode, GuidKey);
            if (info == null)
            {
                // do read, so it creates the record and do a new read.
                info = GetRecord(GuidKey, typeCode, systemId, moduleId);
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
                info.SystemId = systemId;
                Update(info);
            }

            return info;
        }


        public SimplisityRecord GetRecord(string typeCode, int ItemId, int systemId = -1, int moduleId = -1, bool readOnly = false)
        {
            var info = GetRecord(ItemId);
            if (info == null && !readOnly)
            {
                // create record if not in DB
                info = new SimplisityInfo();
                info.GUIDKey = "";
                info.TypeCode = typeCode;
                info.SystemId = systemId;
                info.ModuleId = moduleId;
                info.PortalId = PortalSettings.Current.PortalId;
                info.ItemID = Update(info);
            }
            return GetRecord(info.ItemID);
        }

        public SimplisityRecord SaveRecord(SimplisityRecord sRecord, int systemId)
        {
            var info = GetRecord(sRecord.ItemID);
            if (info == null)
            {
                // do read, so it creates the record and do a new read.
                info = GetRecord(sRecord.TypeCode, sRecord.ItemID, systemId, sRecord.ModuleId);
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
                info.SystemId = systemId;
                Update(info);
                //CacheUtils.ClearAllCache(); // clear ALL cache.
                info = GetRecord(info.ItemID);
            }

            return info;
        }


        #endregion


    }

}
