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

        #region "Fields"

        private SettingsData GetFieldsData(int moduleid = 0)
        {
            if (moduleid == 0) moduleid = _moduleid;
            var settingsData = new SettingsData(_tabid, moduleid, _editLang, "ROCKETMODFIELDS", "fielddata", false, _rocketInterface.DatabaseTable);
            return settingsData;
        }

        private String EditFieldsData()
        {
            try
            {
                var fieldsData = GetFieldsData();
                var strOut = "";
                var passSettings = _paramInfo.ToDictionary();
                var razorTempl = RenderRazorUtils.GetRazorTemplateData(_rocketInterface.DefaultTemplate, _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, _editLang, "1.0", true);
                strOut = RenderRazorUtils.RazorDetail(razorTempl, fieldsData, passSettings, null, true);

                if (strOut == "") strOut = "ERROR: No data returned for EditfieldsData() : " + _rocketInterface.TemplateRelPath + "/Themes/" + _rocketInterface.DefaultTheme + "/default/" + _rocketInterface.DefaultTemplate;
                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        private string FieldsDelete()
        {
            var fieldsData = GetFieldsData();
            fieldsData.Delete();
            return EditFieldsData();
        }

        private string AddFieldsRow()
        {
            var fieldsData = GetFieldsData();
            fieldsData.AddRow();
            return EditFieldsData();
        }

        private String FieldsSave()
        {
            var fieldsData = GetFieldsData();
            fieldsData.Save(_postInfo);

            // AFTER SAVE: calc and save any xpath for fields.
            var objCtrl = new DNNrocketController();
            var dbInfo = objCtrl.GetRecord("ROCKETMODFIELDS", fieldsData.Info.ItemID, -1, true, _rocketInterface.DatabaseTable);
            if (dbInfo != null)
            {
                dbInfo = DNNrocketUtils.UpdateFieldXpath(_postInfo, dbInfo, "fielddata");
                objCtrl.Update(dbInfo, _rocketInterface.DatabaseTable);
            }

            _passSettings.Add("saved", "true");
            return EditFieldsData();
        }

        #endregion



    }
}
