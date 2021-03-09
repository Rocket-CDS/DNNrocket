using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Components
{
    public class LicenseListData
    {
        private DNNrocketController _objCtrl;
        public LicenseListData()
        {
            _objCtrl = new DNNrocketController();
            LoadData();
        }

        private void LoadData()
        {
            List = new List<LicenseData>();
            var l = _objCtrl.GetList(PortalUtils.GetPortalId(), -1, "LICENSE");
            foreach (var d in l)
            {
                var licenseData = new LicenseData(d.ItemID);
                List.Add(licenseData);
            }
        }

        public List<LicenseData> List { get; set;}
    }
}
