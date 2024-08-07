﻿using DNNrocketAPI;
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

            //_searchFilter = " and not(R1.PortalId = 0) ";
            _searchFilter = "";
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
            if (SessionParamData.RowCount == 0)
            {
                // We have no portal record for portal/0, create it.
                var p = new PortalLimpet(0);
                p.Save(new SimplisityInfo());
                SessionParamData.RowCount = _objCtrl.GetListCount(-1, -1, EntityTypeCode, _searchFilter);
            }
            if (UserUtils.IsSuperUser())
            {
                PortalList = _objCtrl.GetList(-1, -1, EntityTypeCode, _searchFilter, "", " order by R1.PortalId", 0, SessionParamData.Page, SessionParamData.PageSize, SessionParamData.RowCount);
            }
            else
            {
                var uId = SessionParamData.GetInt("userid");
                if (uId == 0) uId = UserUtils.GetCurrentUserId();
                PortalList = _objCtrl.GetList(-1, -1, EntityTypeCode, " and UserId = " + uId + " ", "", " order by R1.PortalId", 0, 0, 0, 0);
            }
        }
        public void Validate()
        {
            var pList = PortalUtils.GetAllPortalRecords();
            foreach (var p in pList)
            {
                var pData = new PortalLimpet(p.PortalId);
                pData.Update(); // validate is also done.
            }
            Populate();
        }
        public void Index()
        {
            var pList = PortalUtils.GetAllPortalRecords();
            foreach (var p in pList)
            {
                var pData = new PortalLimpet(p.PortalId);
                pData.ReIndex();
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
