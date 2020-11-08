using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketEcommerce.Componants
{
    public class UserLimpetList
    {
        private List<UserLimpet> _orderList;
        private DNNrocketController _objCtrl;
        private string _searchFilter;
        public UserLimpetList(int portalId, SimplisityInfo paramInfo, bool populate)
        {
            PortalId = portalId;
            if (PortalId == -1) PortalId = PortalUtils.GetCurrentPortalId();

            _objCtrl = new DNNrocketController();

            SessionParamData = new SessionParams(paramInfo);
            if (SessionParamData.PageSize == 0) SessionParamData.PageSize = 32;

            if (populate) Populate();
        }
        public void Populate()
        {
            _searchFilter += "";
            SessionParamData.RowCount = _objCtrl.GetListCount(PortalId, -1, EntityTypeCode, _searchFilter, "");
            UserList = _objCtrl.GetList(PortalId, -1, EntityTypeCode, _searchFilter, "", " order by R1.UserId", 0, SessionParamData.Page, SessionParamData.PageSize, SessionParamData.RowCount);
        }
        public SessionParams SessionParamData { get; set; }
        public List<SimplisityInfo> UserList { get; set; }
        public List<UserLimpet> GetUserList()
        {
            _orderList = new List<UserLimpet>();
            foreach (var o in UserList)
            {
                var orderData = new UserLimpet(o.PortalId, o.UserId);
                _orderList.Add(orderData);
            }
            return _orderList;
        }
        public string ClientImageFolderMapPath { get; set; }
        public string CultureCode { get; private set; }
        public string EntityTypeCode { get { return "USER"; } }
        public int PortalId { get; set; }

    }
}
