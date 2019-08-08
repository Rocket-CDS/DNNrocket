using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Rocket.AppThemes.Componants
{

    public class AppThemeDataList
    {
        public const string AppProjectFolderRel = "/DesktopModules/DNNrocket/AppThemes";

        public AppThemeDataList()
        {
            AssignFolders();
            Populate();
        }
        private void AssignFolders()
        {
            AppProjectAdminFolderRel = AppProjectFolderRel + "/Admin";
            AppProjectThemesFolderRel = AppProjectFolderRel + "/Themes";
            AppProjectThemesFolderMapPath = DNNrocketUtils.MapPath(AppProjectThemesFolderRel);
            AppProjectAdminThemesFolderRel = AppProjectFolderRel + "/Admin/Themes";
            AppProjectAdminThemesFolderMapPath = DNNrocketUtils.MapPath(AppProjectAdminThemesFolderRel);
        }
        public void Populate()
        {
            List = new List<AppTheme>();
            var dirlist = System.IO.Directory.GetDirectories(AppProjectThemesFolderMapPath);
            foreach (var d in dirlist)
            {
                var dr = new System.IO.DirectoryInfo(d);
                var appTheme = new AppTheme(dr.Name);
                List.Add(appTheme);
            }

        }
        public string AppProjectAdminFolderRel { get; set; }
        public string AppProjectAdminThemesFolderRel { get; set; }
        public string AppProjectAdminThemesFolderMapPath { get; set; }
        public string AppProjectThemesFolderRel { get; set; }
        public string AppProjectThemesFolderMapPath { get; set; }

        public List<AppTheme> List { set; get; }


    }

}
