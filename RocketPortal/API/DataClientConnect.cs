using DNNrocketAPI.Components;
using RocketPortal.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPortal.API
{
    public partial class StartConnect
    {

        private String GetDataClientList()
        {
            var dataClientList = new DataClientLimpetList(_paramInfo);
            var razorTempl = _appThemeSystem.GetTemplate("DataClientList.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, dataClientList, null, _passSettings, _sessionParams, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }
        private String GetDataClientRegister()
        {
            var sk = _paramInfo.GetXmlProperty("genxml/remote/securitykey");
            if (_portalData.SecurityKey != sk) return "";
            var siteKey = _paramInfo.GetXmlProperty("genxml/remote/sitekey");
            var dataClient = new DataClientLimpet(PortalUtils.GetCurrentPortalId(), siteKey);
            dataClient.UrlApi = _paramInfo.GetXmlProperty("genxml/remote/apiurl");
            dataClient.UrlWithProtocol = _paramInfo.GetXmlProperty("genxml/remote/url");
            dataClient.Update();
            return "OK";
        }        
        private String ResetSecuirtyDataClient()
        {
            var siteKey = _paramInfo.GetXmlProperty("genxml/hidden/sitekey");
            var portalid = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            var dataClient = new DataClientLimpet(portalid, siteKey);
            if (dataClient.Exists) dataClient.ResetSecurity();
            return GetDataClientList();
        }
        private String DeleteDataClient()
        {
            var siteKey = _paramInfo.GetXmlProperty("genxml/hidden/sitekey");
            var portalid = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            var dataClient = new DataClientLimpet(portalid, siteKey);
            if (dataClient.Exists) dataClient.Delete();
            return GetDataClientList();
        }
        private String DataClientActive()
        {
            var siteKey = _paramInfo.GetXmlProperty("genxml/hidden/sitekey");
            var portalid = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            var dataClient = new DataClientLimpet(portalid, siteKey);
            if (dataClient.Active)
                dataClient.Active = false;
            else
                dataClient.Active = true;
            dataClient.Update();
            return GetDataClientList();
        }

    }
}
