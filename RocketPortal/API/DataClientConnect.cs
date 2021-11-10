﻿using DNNrocketAPI.Components;
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
            return RenderRazorUtils.RazorDetail(razorTempl, dataClientList, _passSettings, _sessionParams, true);
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