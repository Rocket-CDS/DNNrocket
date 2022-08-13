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

        private String GetProjectList()
        {
            var orgData = new AppThemeProjectLimpet();
            var razorTempl = _appThemeSystem.GetTemplate("GitHubRepoList.cshtml");

            var pr = RenderRazorUtils.RazorProcessData(razorTempl, orgData, null, _passSettings, _sessionParams, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }
        private String AddProject()
        {
            var orgData = new AppThemeProjectLimpet();
            orgData.AddRow();
            return GetProjectList();
        }
        private String SaveProject()
        {
            var orgData = new AppThemeProjectLimpet();
            orgData.Record.XMLData = _postInfo.XMLData;
            orgData.Update();
            return GetProjectList();
        }
        private String DeleteProject()
        {
            var idx = _paramInfo.GetXmlPropertyInt("genxml/hidden/idx") -1;
            var orgData = new AppThemeProjectLimpet();
            orgData.DeleteRow(idx);
            return GetProjectList();
        }
    }
}
