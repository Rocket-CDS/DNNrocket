using DNNrocketAPI;
using RocketSettings;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace RocketMod
{

    public class ModuleData
    {

        private List<SimplisityInfo> _dataList;
        private int _tabid;
        private int _moduleid;
        private int _systemid;
        private int _selecteditemid;
        private int _status;
        private string _message;
        private string _entityTypeCode;
        private string _langRequired;


        private SettingsData _productData;
        private ConfigData _configData;
        private SimplisityInfo _headerInfo;
        private SimplisityInfo _auditInfo;
        private SimplisityInfo _currentRecord;

        public ModuleData(string langRequired = "")
        {
            _langRequired = langRequired;
            if (_langRequired == "") _langRequired = DNNrocketUtils.GetCurrentCulture();
            _currentRecord = new SimplisityInfo(_langRequired);
        }

        public ModuleData(ConfigData configData, string langRequired = "")
        {
            _langRequired = langRequired;
            if (_langRequired == "") _langRequired =  DNNrocketUtils.GetCurrentCulture();
            _entityTypeCode = "ROCKETMOD";
            _tabid = configData.TabId;
            _moduleid = configData.ModuleId;
            _dataList = new List<SimplisityInfo>();
            _systemid = configData.SystemId;
            _configData = configData;

            if (_configData.Exists)
            {
                PopulateHeader();
                _productData = new SettingsData(_tabid, _moduleid, _langRequired, _entityTypeCode);
                Populate(_productData.Info.ItemID);
            }
        }

        #region "PRODUCTS"

        public void PopulateProducts()
        {
            _productData = new SettingsData(_tabid, _moduleid, _langRequired, _entityTypeCode);
        }

        public SimplisityInfo GetProduct(string productref)
        {
            return _productData.Info.GetListItem("settingsdata", "genxml/textbox/productref", productref);
        }

        public List<SimplisityInfo> GetProducts()
        {
            return _productData.List;
        }

        #endregion

        #region "HEADER"

        public void PopulateHeader()
        {
            var objCtrl = new DNNrocketController();
            _headerInfo = objCtrl.GetData("rocketmod_" + _moduleid, "HEADER", _langRequired, -1, _moduleid, true);
            if (_headerInfo == null)
            {
                _headerInfo = new SimplisityInfo();
                _headerInfo.ModuleId = _moduleid;
            }
        }

        public void DeleteHeader()
        {
            var objCtrl = new DNNrocketController();
            var info = objCtrl.GetData("rocketmod_" + _moduleid, "HEADER", _langRequired, -1, _moduleid, true);
            if (info != null)
            {
                objCtrl.Delete(info.ItemID);
                ClearCache();
                if (_configData.Exists)
                {
                    PopulateHeader();
                    PopulateList(); // search data is saved in the header so we need to reload.
                }
            }
        }

        public void SaveHeader(SimplisityInfo postInfo)
        {
            //remove any params
            postInfo.RemoveXmlNode("genxml/postform");
            postInfo.RemoveXmlNode("genxml/urlparams");

            var objCtrl = new DNNrocketController();
            var info = objCtrl.SaveData("rocketmod_" + _moduleid, "HEADER", postInfo, -1, _moduleid);
            ClearCache();
            if (_configData.Exists)
            {
                PopulateHeader();
                PopulateList(); // search data is saved in the header so we need to reload.
            }
        }

        #endregion

        #region "AUDIT"

        public void PopulateAudit()
        {
            if (_currentRecord != null && _currentRecord.ItemID > 0 && _currentRecord.TypeCode == "PAY")
            {
                var objCtrl = new DNNrocketController();
                var auditRecord = objCtrl.GetByGuidKey(-1,_moduleid,"PAYAUDIT", _currentRecord.GUIDKey);
                if (auditRecord == null)
                {
                    _auditInfo = new SimplisityInfo();
                    _auditInfo.ItemID = -1;
                    _auditInfo.GUIDKey = _currentRecord.GUIDKey;
                    _auditInfo.TypeCode = "PAYAUDIT";
                    _auditInfo.ModuleId = _moduleid;
                    _auditInfo.PortalId = _currentRecord.PortalId;
                    _auditInfo.ParentItemId = _currentRecord.ItemID;
                    _auditInfo.SystemId = _currentRecord.SystemId;
                    objCtrl.Update(_auditInfo);
                }
                else
                {
                    _auditInfo = objCtrl.GetInfo(auditRecord.ItemID, _langRequired);
                }

            }

        }

        public void AddAudit()
        {
            if (_currentRecord != null && _currentRecord.ItemID > 0 && _auditInfo != null && _auditInfo.ItemID > 0)
            {
                var objCtrl = new DNNrocketController();
                _currentRecord.SetXmlProperty("genxml/hidden/auditdate", DateTime.Now.ToString("s"));
                _auditInfo.AddListRow("audit", _currentRecord);
                objCtrl.Update(_auditInfo);
                PopulateAudit();
            }
        }

        public List<SimplisityInfo> GetAuditList()
        {
            if (_auditInfo == null) return new List<SimplisityInfo>();
            return _auditInfo.GetList("audit");
        }

        #endregion


        #region "Data"



        public void Populate(int itemId, string guidkey = "")
        {
            if (itemId > 0)
            {
                if (guidkey == "")
                {
                    PopulateCurrentData(itemId);
                    PopulateAudit();
                }
                else
                {
                    var objCtrl = new DNNrocketController();
                    var securityRecord = objCtrl.GetInfo(itemId);
                    if (securityRecord != null && securityRecord.GUIDKey == guidkey)
                    {
                        PopulateCurrentData(itemId);
                        PopulateAudit();
                    }
                    else
                    {
                        _currentRecord = new SimplisityInfo(_langRequired);
                    }
                }
            }
            else
            {
                _currentRecord = new SimplisityInfo(_langRequired);
            }
        }

        public void PopulateList(int page = 1, int pagesize = 20)
        {
            PopulateDataList(page, pagesize);
        }

        private void PopulateCurrentData(int itemId)
        {
            _selecteditemid = itemId;
            if (_moduleid > 0)
            {
                var objCtrl = new DNNrocketController();

                _currentRecord = objCtrl.GetInfo(_selecteditemid, _langRequired);
                if (_currentRecord == null) _currentRecord = new SimplisityInfo(_langRequired);
                _currentRecord.Lang = _langRequired;  // we need the language to format data.

            }
        }

        private void PopulateDataList(int page, int pagesize)
        {
            if (_moduleid > 0)
            {
                var _searchInfo = _headerInfo;

                var objCtrl = new DNNrocketController();
                var filter = "";
                _searchInfo.SetXmlProperty("genxml/searchflag", "False");
                    if (_searchInfo.GetXmlProperty("genxml/hidden/searchtext") != "")
                    {
                        filter += " and (";
                        filter += " NAMEIDX.GuidKey like '%" + _searchInfo.GetXmlProperty("genxml/hidden/searchtext") + "%' ";
                        filter += " or ITEMREFIDX.GuidKey like '%" + _searchInfo.GetXmlProperty("genxml/hidden/searchtext") + "%' ";
                    filter += " or EMAILIDX.GuidKey like '%" + _searchInfo.GetXmlProperty("genxml/hidden/searchtext") + "%' ";
                    filter += " )";
                        _searchInfo.SetXmlProperty("genxml/searchflag", "True");
                    }
                if (_searchInfo.GetXmlProperty("genxml/hidden/status") != "")
                {
                    filter += " and STATUSIDX.GuidKey = '" + _searchInfo.GetXmlProperty("genxml/hidden/status") + "' ";
                    _searchInfo.SetXmlProperty("genxml/searchflag", "True");
                }
                else
                {
                    filter += " and not ( STATUSIDX.GuidKey = '3') ";
                }

                var rowcount = objCtrl.GetListCount(_configData.ConfigInfo.PortalId, _moduleid, "PAY", filter, "", _systemid);

                _searchInfo.SetXmlProperty("genxml/hidden/rowcount", rowcount.ToString()); // paging template needs this

                _dataList = objCtrl.GetList(-1, _moduleid, "PAY", filter, "", " order by R1.ItemId DESC", 0, page, pagesize, rowcount, _systemid);

                // save paging to header.
                _headerInfo.SetXmlProperty("genxml/hidden/page", page.ToString());
                _headerInfo.SetXmlProperty("genxml/hidden/pagesize", pagesize.ToString());
                objCtrl.SaveData(_headerInfo, _systemid);

                // setup ajax call for paging
                _searchInfo.SetXmlProperty("genxml/s-paging-cmd", "rocketmod_search");
                _searchInfo.SetXmlProperty("genxml/s-paging-post", "#headerdata");
                _searchInfo.SetXmlProperty("genxml/s-paging-fields", "tabid:" + _tabid + ",moduleid:" + _moduleid + ",selecteditemid:,interfacekey:rocketmod");
                _searchInfo.SetXmlProperty("genxml/s-paging-return", "#simplisity_startpanel");

            }
        }

        private SimplisityInfo AddNewInfo(SimplisityInfo postInfo)
        {
            var objCtrl = new DNNrocketController();

            //remove any params
            postInfo.RemoveXmlNode("genxml/postform");
            postInfo.RemoveXmlNode("genxml/urlparams");

            // get product list, to get qty and productref (used to read product data)
            var productlist = postInfo.GetList("productlistitem");
            postInfo.RemoveXmlNode("genxml/productlistitem");

            var info = new SimplisityInfo();
            info.ItemID = -1;
            info.PortalId = DNNrocketUtils.GetPortalId();
            info.Lang = _langRequired;
            info.TypeCode = "PAY";
            info.ModuleId = _moduleid;
            info.GUIDKey = Guid.NewGuid().ToString("");
            info.SystemId = _systemid;
            info.XMLData = postInfo.XMLData;

            info.SetXmlProperty("genxml/status", "0");
            info.SetXmlProperty("genxml/hidden/paydate", DateTime.Now.ToString("s"),TypeCode.DateTime);

            // remove zero qty products
            foreach (var i in productlist)
            {
                if (i.GetXmlPropertyInt("genxml/textbox/qty") > 0)
                {
                    info.AddListRow("productlistitem", i);
                }
            }

            var newRecord = objCtrl.SaveRecord(info, _systemid);
            newRecord.SetXmlProperty("genxml/hidden/vouchernumber", DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Day.ToString("00") + "-" + newRecord.ItemID);
            newRecord = objCtrl.SaveRecord(newRecord, _systemid);

            var newInfo = new SimplisityInfo(newRecord);

            ClearCache();
            return newInfo;
        }

        public void AddCurrentInfo(SimplisityInfo postInfo)
        {
            _currentRecord = AddNewInfo(postInfo);
            _selecteditemid = _currentRecord.ItemID;

        }

        public void PurgeArchived()
        {
            if (_moduleid > 0)
            {
                var _searchInfo = _headerInfo;

                var objCtrl = new DNNrocketController();
                var filter = " and STATUSIDX.GuidKey = '3' ";
                _dataList = objCtrl.GetList(-1, _moduleid, "PAY", filter, "", "", 0, 0, 0, 0, _systemid);
                foreach (var i in _dataList)
                {
                    objCtrl.Delete(i.ItemID);
                }
            }
        }


        public void DeleteData()
        {
            if (_moduleid > 0)
            {
                if (_selecteditemid > 0)
                {
                    DeleteCurrentRecord();
                }
                else
                {
                    var objCtrl = new DNNrocketController();
                    var filter = "";
                    _dataList = objCtrl.GetList(-1, _moduleid, "PAY", filter, "", "", 0, 0, 0, 0);
                    foreach (var i in _dataList)
                    {
                        objCtrl.Delete(i.ItemID);
                    }
                    ClearCache();
                    _dataList = objCtrl.GetList(-1, _moduleid, "PAY", filter, "", "", 0, 0, 0, 0);
                    PopulateList();
                }
            }
        }

        public void DeleteCurrentRecord()
        {
            if (_moduleid > 0 && _selecteditemid > 0)
            {
                var objCtrl = new DNNrocketController();
                objCtrl.Delete(_selecteditemid);
                _selecteditemid = 0;
                ClearCache();
                PopulateList();
            }
        }

        public void SaveData(bool createAudit = true)
        {
            if (_currentRecord != null && _currentRecord.ItemID > 0 && _currentRecord.TypeCode == "PAY")
            {
                SaveData(_currentRecord, createAudit);
            }
        }


        public void SaveData(SimplisityInfo postInfo, bool createAudit = true)
        {
            if (_moduleid > 0)
            {
                if (_selecteditemid <= 0)
                {
                    _selecteditemid = postInfo.GetXmlPropertyInt("genxml/hidden/selecteditemid");
                }
                if (_selecteditemid > 0)
                {
                    Populate(_selecteditemid);
                    //remove any params
                    postInfo.RemoveXmlNode("genxml/postform");
                    postInfo.RemoveXmlNode("genxml/urlparams");

                    var objCtrl = new DNNrocketController();
                    if (_currentRecord != null && _currentRecord.ItemID > 0 && _currentRecord.TypeCode == "PAY")
                    {

                        var username = UserUtils.GetCurrentUserName();
                        if (username == "") username = "---";

                        var info = _currentRecord;

                        if (createAudit)
                        {
                            AddAudit();
                        }

                        info.SetXmlProperty("genxml/status", postInfo.GetXmlProperty("genxml/status"));
                        info.SetXmlProperty("genxml/username", username);
                        info.SetXmlProperty("genxml/textbox/notes", postInfo.GetXmlProperty("genxml/textbox/notes"));
                        if (GeneralUtils.IsEmail(postInfo.GetXmlProperty("genxml/textbox/email")))
                        {
                            info.SetXmlProperty("genxml/textbox/email", postInfo.GetXmlProperty("genxml/textbox/email"));
                        }
                        if (postInfo.GetXmlProperty("genxml/textbox/name") != "")
                        {
                            info.SetXmlProperty("genxml/textbox/name", postInfo.GetXmlProperty("genxml/textbox/name"));
                        }
                        info.SetXmlProperty("genxml/hidden/orderemailsent", postInfo.GetXmlProperty("genxml/hidden/orderemailsent"));
                        info.SetXmlProperty("genxml/hidden/voucheremailsent", postInfo.GetXmlProperty("genxml/hidden/voucheremailsent"));

                        info.ModuleId = _moduleid;

                        info.SetXmlProperty("genxml/textbox/message", _message);
                        info.Lang = "";  //Language only needed for formating, no need to save.
                        objCtrl.SaveRecord(info, _systemid);
                    }
                    ClearCache();
                }
            }
        }


        #endregion

        #region "actions"

        public void SetStatus(int orderStatus)
        {
            if (_currentRecord != null && _currentRecord.ItemID > 0 && _currentRecord.TypeCode == "PAY")
            {
                _currentRecord.SetXmlProperty("genxml/status", orderStatus.ToString());
                SaveData(_currentRecord);
            }
        }

        public void EmptyList()
        {
            _dataList = new List<SimplisityInfo>();
        }


        #endregion

        #region "properties"

        public int ModuleId { get {return _moduleid;} }
        public int TabId { get { return _tabid; } }
        public int SelectedItemId { get { return _selecteditemid; } }

        public SimplisityInfo CurrentRecord { get { return _currentRecord; } }
        public SimplisityInfo HeaderInfo { get { return _headerInfo; } }
        public SettingsData productData { get { return _productData; } }
        public ConfigData configData { get { return _configData; } }        

        public int Status { get { return _status; } }

        public string Message { get { return _message; } set { _message = value; } }
        public string ProductEntityTypeCode { get { return _entityTypeCode; } }        

        public List<SimplisityInfo> List
        {
            get { return _dataList; }
        }

        #endregion


        #region "EMAIL"

        public void SendEmailVoucher(string appthemeRelPath, bool forceSend = false)
        {
            SendEmail(appthemeRelPath, true, forceSend);
        }

        public void SendEmailOrder(string appthemeRelPath, bool forceSend = false)
        {
            SendEmail(appthemeRelPath, false, forceSend);
        }

        private void SendEmail(string appthemeRelPath, bool isVoucher = false, bool forceSend = false)
        {
            if (forceSend || (CurrentRecord.GetXmlProperty("genxml/hidden/orderemailsent") == ""))
            {

                var razortemplate = "EmailOrder.cshtml";
                var subrazortemplate = DNNrocketUtils.MapPath(appthemeRelPath).TrimEnd('\\') + "\\Themes\\config-w3\\default\\" + _configData.ConfigInfo.GetXmlProperty("genxml/textbox/razortemplate").Replace(".cshtml", "_emailorder.cshtml");
                if (isVoucher)
                {
                    razortemplate = "EmailVoucher.cshtml";
                    subrazortemplate = DNNrocketUtils.MapPath(appthemeRelPath).TrimEnd('\\') + "\\Themes\\config-w3\\default\\" + _configData.ConfigInfo.GetXmlProperty("genxml/textbox/razortemplate").Replace(".cshtml", "_voucher.cshtml");
                }
                if (File.Exists(subrazortemplate))
                {
                    var passSettings = _configData.ConfigInfo.ToDictionary();
                    var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, appthemeRelPath, "config-w3", _langRequired);
                    var emailbody = DNNrocketUtils.RazorDetail(razorTempl, this, passSettings, HeaderInfo);
                    var toEmail = "";
                    if (GeneralUtils.IsEmail(CurrentRecord.GetXmlProperty("genxml/textbox/email")))
                    {
                        toEmail += CurrentRecord.GetXmlProperty("genxml/textbox/email");
                    }
                    var emailsubject = DNNrocketUtils.GetResourceString("/DesktopModules/DNNrocketModules/rocketmod/App_LocalResources/", "rocketmod.voucher") + " : " + _configData.WebsiteUrl + " : " + _configData.CompanyName;
                    if (!isVoucher)
                    {
                        toEmail = "," + configData.ManagerEmail;
                        emailsubject = DNNrocketUtils.GetResourceString("/DesktopModules/DNNrocketModules/rocketmod/App_LocalResources/", "rocketmod.order") + " : " + _configData.WebsiteUrl + " : " + _configData.CompanyName;
                        CurrentRecord.SetXmlProperty("genxml/hidden/voucheremailsent", DateTime.Now.ToString("s"), TypeCode.DateTime);
                    }
                    CurrentRecord.SetXmlProperty("genxml/hidden/orderemailsent", DateTime.Now.ToString("s"), TypeCode.DateTime);

                    var fromEmail = _configData.ManagerEmail;

                    string lang = "";
                    string attchments = "";

                    SaveData(false);

                    EmailUtils.SendEmail(emailbody, toEmail, emailsubject, fromEmail, lang, attchments);
                }
            }
        }

        #endregion

        public void ClearCache()
        {
            CacheUtils.ClearCache("rocketmod" + _moduleid);
        }




    }

}
