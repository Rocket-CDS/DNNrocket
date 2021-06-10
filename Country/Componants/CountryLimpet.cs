using DNNrocketAPI;
using DNNrocketAPI.Components;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNNrocket.Country.Components
{
    public class CountryLimpet
    {
        private DNNrocketController _objCtrl;
        public const string _tableName = "DNNrocket";
        public const string _entityTypeCode = "COUNTRY";

        public CountryLimpet(int portalId, RocketInterface rocketInterface, string cultureCode = "")
        {
            RocketInterface = rocketInterface;
            if (cultureCode == "") cultureCode = DNNrocketUtils.GetEditCulture();
            CultureCode = cultureCode;
            _objCtrl = new DNNrocketController();

            Info = _objCtrl.GetByGuidKey(portalId, -1, _entityTypeCode, RocketInterface.InterfaceKey, "", _tableName);
            if (Info == null || Info.ItemID <= 0)
            {
                Info = new SimplisityInfo();
                Info.PortalId = portalId;
                Info.ModuleId = -1;
                Info.TypeCode = _entityTypeCode;
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
            _objCtrl.SaveData(Info, _tableName);
        }
        public void Delete()
        {
            _objCtrl.Delete(Info.ItemID, _tableName);
        }

        public List<SimplisityInfo> GetSelectedCountries()
        {
            return Info.GetList("countrylist");
        }

        public string GetPrimaryCountry()
        {
            var i = Info.GetListItem("countrylist",0);
            return i.GetXmlProperty("genxml/hidden/countrycode");
        }

        public Dictionary<string,string> GetSelectedDictCountries()
        {
            var clist = DNNrocketUtils.GetCountryCodeList();
            var rtn = new Dictionary<string, string>();
            foreach (var i in Info.GetList("countrylist"))
            {
                var ccode = i.GetXmlProperty("genxml/hidden/countrycode");
                var countryname = "";
                if (clist.ContainsKey(ccode)) countryname = clist[ccode];
                rtn.Add(ccode,countryname);
            }
            return rtn;
        }
        public string CountryName(string countrycode)
        {
            return DNNrocketUtils.GetCountryName(countrycode);
        }
        public Dictionary<string,string> RegionDictionary(string countrycode)
        {
            return DNNrocketUtils.GetRegionList(countrycode);
        }
        public Dictionary<string, string> CountryCodeList()
        {
            return DNNrocketUtils.GetCountryCodeList(PortalId);
        }

        public SimplisityInfo Info { get; set; }
        public RocketInterface RocketInterface { get; set; }
        public int PortalId { get { return Info.PortalId; } }
        public string CultureCode { get; set; }

    }
}
