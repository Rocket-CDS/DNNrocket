using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Componants
{
    public class LicenseListData
    {
        private DNNrocketController _objCtrl;
        public LicenseListData()
        {
            _objCtrl = new DNNrocketController();
            LoadData();
        }

        public void AddLicense(int portalId, string systemKey)
        {
            var licenseData = new LicenseData(portalId, systemKey);
            licenseData.Update();
            LoadData();
        }

        private void LoadData()
        {
            List = new List<LicenseData>();
            var l = _objCtrl.GetList(DNNrocketUtils.GetPortalId(), -1, "LICENSE");
            foreach (var d in l)
            {
                var r = new SimplisityRecord(d);
                var licenseData = new LicenseData(r);
                List.Add(licenseData);
            }
        }

        public List<LicenseData> List { get; set;}
    }
}
