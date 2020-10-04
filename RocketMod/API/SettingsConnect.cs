using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Rocket.AppThemes.Componants;
using System;
using System.Collections.Generic;
using System.Text;

namespace RocketMod
{
    public partial class StartConnect
    {
        private SettingsData GetSettingsData()
        {
            return new SettingsData(_tabid, _moduleid, _editLang, "ROCKETMODSETTINGS", "rocketmodsettings", true, _rocketInterface.DatabaseTable);
        }
        private String EditSettingsData()
        {
            try
            {
                var settingsData = GetSettingsData();
                var strOut = "";
                var passSettings = _paramInfo.ToDictionary();
                var razorTempl = RenderRazorUtils.GetRazorTemplateData(_rocketInterface.DefaultTemplate, _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetEditCulture(), "1.0", true);
                strOut = RenderRazorUtils.RazorDetail(razorTempl, settingsData, passSettings);

                if (strOut == "") strOut = "ERROR: No data returned for EditSettingsData() : " + _rocketInterface.TemplateRelPath + "/Themes/" + _rocketInterface.DefaultTheme + "/default/" + _rocketInterface.DefaultTemplate;
                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        private string SettingsDelete()
        {
            var settingsData = GetSettingsData();
            settingsData.Delete();
            CacheFileUtils.ClearAllCache();
            return EditSettingsData();
        }

        private string AddSettingsRow()
        {
            var settingsData = GetSettingsData();
            settingsData.AddRow();
            CacheFileUtils.ClearAllCache();
            return EditSettingsData();
        }

        private String SettingsSave()
        {
            var settingsData = GetSettingsData();
            settingsData.Save(_postInfo);
            CacheFileUtils.ClearAllCache();
            return EditSettingsData();
        }
        public String GetSettingSection()
        {
            try
            {
                var appTheme = new AppTheme(_systemData.SystemKey, _moduleParams.AppThemeFolder, _moduleParams.AppThemeVersion);
                if (!appTheme.EnableSettings) return "";
                var razorTempl = appTheme.GetTemplate("settings.cshtml"); // new module, so settings theme will be systemtheme.
                _settingsData = GetSettingsData();
                return RenderRazorUtils.RazorDetail(razorTempl, _settingsData, _passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }


    }
}
