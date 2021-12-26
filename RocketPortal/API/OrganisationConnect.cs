using DNNrocketAPI.Components;
using Rocket.AppThemes.Components;
using RocketPortal.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPortal.API
{
    public partial class StartConnect
    {

        private String GetOrgList()
        {
            var orgData = new OrganisationLimpet();
            var razorTempl = _appThemeSystem.GetTemplate("OrganisationList.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, orgData, null, _passSettings, _sessionParams, true);
            return pr.RenderedText;
        }
        private String AddOrg()
        {
            var orgData = new OrganisationLimpet();
            orgData.AddRow();
            return GetOrgList();
        }
        private String SaveOrg()
        {
            var orgData = new OrganisationLimpet();
            orgData.Record.XMLData = _postInfo.XMLData;
            orgData.Update();
            return GetOrgList();
        }
        private String DeleteOrg()
        {
            var idx = _paramInfo.GetXmlPropertyInt("genxml/hidden/idx");
            var orgData = new OrganisationLimpet();
            orgData.DeleteRow(idx);
            return GetOrgList();
        }
    }
}
