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
    public class PortalLimpetList
    {
        private List<PortalLimpet> _portalList;
        private DNNrocketController _objCtrl;
        private string _searchFilter;

        public PortalLimpetList(SimplisityInfo paramInfo)
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
                PortalList = _objCtrl.GetList(-1, -1, EntityTypeCode, _searchFilter, "", " order by R1.PortalId", 0, SessionParamData.Page, SessionParamData.PageSize, SessionParamData.RowCount);
            }
            else
            {
                PortalList = _objCtrl.GetList(-1, -1, EntityTypeCode, " and UserId = " + UserUtils.GetCurrentUserId() + " ", "", " order by R1.PortalId", 0, 0, 0, 0);
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
        public List<SimplisityInfo> PortalList { get; set; }
        public List<PortalLimpet> GetPortalList()
        {
            _portalList = new List<PortalLimpet>();
            foreach (var o in PortalList)
            {
                var portalData = new PortalLimpet(o.PortalId);
                _portalList.Add(portalData);
            }
            return _portalList;
        }
        public string CultureCode { get; private set; }
        public string SystemKey { get { return "rocketportal"; } }
        public string EntityTypeCode { get { return "PORTAL"; } }

    }
}
