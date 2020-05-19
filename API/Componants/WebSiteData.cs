using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Componants
{
    public class WebSiteData
    {
        private static string _cacheKey;
        private static int _portalid;
        public WebSiteData(bool cache = true)
        {
            _portalid = PortalUtils.GetPortalId();
            _cacheKey = "rocketWebSiteData" + _portalid;
            if (cache) Info = (SimplisityInfo)CacheUtilsDNN.GetCache(_cacheKey);
            if (Info == null) LoadData();
        }
        public WebSiteData(int Portalid, bool cache = true)
        {
            _portalid = Portalid;
            _cacheKey = "rocketWebSiteData" + _portalid;
            if (cache) Info = (SimplisityInfo)CacheUtilsDNN.GetCache(_cacheKey);
            if (Info == null) LoadData();
        }

        public void Save(SimplisityInfo postInfo)
        {
            Info.XMLData = postInfo.XMLData;
            Update();
        }
        public void Update()
        {
            var objCtrl = new DNNrocketController();
            objCtrl.Update(Info);
            CacheUtilsDNN.RemoveCache(_cacheKey);
            CacheUtilsDNN.SetCache(_cacheKey, Info);
        }

        private void LoadData()
        {
            var objCtrl = new DNNrocketController();
            Info = objCtrl.GetByType(_portalid, -1, "WEBSITEDATA");
            if (Info == null)
            {
                Info = new SimplisityInfo();
                Info.ItemID = -1;
                Info.PortalId = _portalid;
                Info.TypeCode = "WEBSITEDATA";

                // add any MUST have settings.
                Info.SetXmlProperty("genxml/portalid", _portalid.ToString());

                Info.ItemID = objCtrl.Update(Info);
            }
            CacheUtilsDNN.SetCache(_cacheKey, Info);
        }

        public SimplisityInfo Info { get; set; }

        public string CompanyKey { get { return Info.GetXmlProperty("genxml/companykey"); } set { Info.SetXmlProperty("genxml/companykey", value); } }


    }
}
