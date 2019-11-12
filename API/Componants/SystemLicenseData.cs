using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Componants
{
    public class SystemLicenseData
    {
        private DNNrocketController _objCtrl;
        public SystemLicenseData()
        {
            _objCtrl = new DNNrocketController();
            LoadData();
        }

        public void Save(SimplisityRecord postInfo)
        {
            LoadData();
        }

        public void AddLicense(SimplisityRecord newLicenseRecord)
        {
            var l = new LicenseData(newLicenseRecord); 
            l.Update();
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
