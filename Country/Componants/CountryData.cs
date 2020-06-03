using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNNrocket.Country.Componants
{
    public class CountryData
    {
        private DNNrocketController _objCtrl;

        public CountryData(int portalId, DNNrocketInterface rocketInterface, string cultureCode = "")
        {
            RocketInterface = rocketInterface;
            SystemKey = RocketInterface.SystemKey;
            if (cultureCode == "") cultureCode = DNNrocketUtils.GetEditCulture();
            CultureCode = cultureCode;
            _objCtrl = new DNNrocketController();

            Info = _objCtrl.GetByGuidKey(portalId, -1, RocketInterface.EntityTypeCode, RocketInterface.InterfaceKey, "", RocketInterface.DatabaseTable);
            if (Info == null || Info.ItemID <= 0)
            {
                Info = new SimplisityInfo();
                Info.PortalId = portalId;
                Info.ModuleId = -1;
                Info.TypeCode = RocketInterface.EntityTypeCode;
                Info.GUIDKey = RocketInterface.InterfaceKey;
                Info.Lang = CultureCode;
            }
        }


        public void Save(SimplisityInfo info)
        {
            Info.XMLData = info.XMLData;
            Update();
        }
        public void Update()
        {
            _objCtrl.SaveData(Info, RocketInterface.DatabaseTable);
        }
        public void Delete()
        {
            _objCtrl.Delete(Info.ItemID, RocketInterface.DatabaseTable);
        }

        public List<SimplisityInfo> GetSelectedCountries()
        {
            return Info.GetList("countrylist");
        }

        public SimplisityInfo Info { get; set; }
        public DNNrocketInterface RocketInterface { get; set; }
        public int PortalId { get { return Info.PortalId; } }
        public string CultureCode { get; set; }
        public string SystemKey { get; set; }

    }
}
