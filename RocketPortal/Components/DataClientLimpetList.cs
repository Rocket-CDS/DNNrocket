using DNNrocketAPI;
using DNNrocketAPI.Components;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketPortal.Components
{
    public class DataClientLimpetList
    {
        private List<DataClientLimpet> _dataClientList;
        private DNNrocketController _objCtrl;
        private string _searchFilter;

        public DataClientLimpetList(SimplisityInfo paramInfo)
        {
            _objCtrl = new DNNrocketController();

            SessionParamData = new SessionParams(paramInfo);
            if (paramInfo.GetXmlPropertyInt("genxml/urlparams/pagesize") != 0) SessionParamData.PageSize = paramInfo.GetXmlPropertyInt("genxml/urlparams/pagesize");
            if (paramInfo.GetXmlPropertyInt("genxml/urlparams/page") != 0) SessionParamData.Page = paramInfo.GetXmlPropertyInt("genxml/urlparams/page");
            if (SessionParamData.PageSize == 0) SessionParamData.PageSize = 32;

            _searchFilter = " and not(R1.PortalId = 0) ";
            if (SessionParamData.SearchText != "")
            {
                _searchFilter = "  and    (";
                _searchFilter += " R1.GuidKey like '%" + SessionParamData.SearchText + "%' ";
                _searchFilter += " or [XMLData].value('(genxml/textbox/name)[1]', 'nvarchar(max)') like '%" + SessionParamData.SearchText + "%'";
                _searchFilter += " or [XMLData].value('(genxml/textbox/engineurl)[1]', 'nvarchar(max)') like '%" + SessionParamData.SearchText + "%'";
                _searchFilter += " )";
            }
            Populate();
        }
        public void Populate()
        {
            SessionParamData.RowCount = _objCtrl.GetListCount(-1, -1, EntityTypeCode, _searchFilter);
            if (UserUtils.IsSuperUser())
            {
                DataClientList = _objCtrl.GetList(-1, -1, EntityTypeCode, _searchFilter, "", " order by R1.PortalId", 0, SessionParamData.Page, SessionParamData.PageSize, SessionParamData.RowCount);
            }
            else
            {
                DataClientList = _objCtrl.GetList(-1, -1, EntityTypeCode, " and UserId = " + UserUtils.GetCurrentUserId() + " ", "", " order by R1.PortalId", 0, 0, 0, 0);
            }
        }
        public void Validate()
        {
            var pList = PortalUtils.GetAllPortalRecords();
            foreach (var p in pList)
            {
                var pData = new PortalLimpet(p.PortalId);
                pData.Validate();
                pData.Update();
            }
            Populate();
        }

        public SessionParams SessionParamData { get; set; }
        public List<SimplisityInfo> DataClientList { get; set; }
        public List<DataClientLimpet> GetDataClientList()
        {
            _dataClientList = new List<DataClientLimpet>();
            foreach (var o in DataClientList)
            {
                var dataClient = new DataClientLimpet(o.PortalId, o.GUIDKey);
                _dataClientList.Add(dataClient);
            }
            return _dataClientList;
        }
        public string CultureCode { get; private set; }
        public string SystemKey { get { return "rocketportal"; } }
        public string EntityTypeCode { get { return "DATACLIENT"; } }

    }
}
