using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Componants
{
    public class BackUpDataList
    {
        private string _searchpattern; 
        public BackUpDataList(string backupRootFolder, string searchpattern)
        {
            _searchpattern = searchpattern;
            BackupRootFolder = backupRootFolder;
            BackupRootFolderMapPath = DNNrocketUtils.BackUpDirectoryMapPath() + "\\" + BackupRootFolder;
            LoadData();
        }

        private void LoadData()
        {
            if (!Directory.Exists(BackupRootFolderMapPath)) Directory.CreateDirectory(BackupRootFolderMapPath);

            List = new Dictionary<string, List<BackUpData>>();
            ListFileMapPath = new Dictionary<string, List<string>>();            
            SystemKeyFolderList = new List<string>();
            var globalSettings = new SystemGlobalData();
            var backuplimit = globalSettings.Info.GetXmlPropertyInt("genxml/textbox/backuplimit");
            if (backuplimit <= 0) backuplimit = 1;

            var l = Directory.GetDirectories(BackupRootFolderMapPath);
            foreach (var d in l)
            {
                string fullPath = Path.GetFullPath(d).TrimEnd(Path.DirectorySeparatorChar);
                string projectName = Path.GetFileName(fullPath);

                var fileList = new List<BackUpData>();
                var fileMapPathList = new List<string>();                
                SystemKeyFolderList.Add(projectName);
                var l2 = Directory.GetFiles(d, _searchpattern).Reverse();
                var lp = 1;
                foreach (var f in l2)
                {
                    if (lp <= backuplimit)
                    {
                        var BackUpData = new BackUpData(f);
                        fileList.Add(BackUpData);
                        fileMapPathList.Add(f);
                    }
                    else
                    {
                        File.Delete(f);
                    }
                    lp += 1;
                }
                List.Add(projectName, fileList);
                ListFileMapPath.Add(projectName, fileMapPathList);
            }
        }

        public List<string> GetBackUpFileMapPathList(string systemKeyFolder)
        {
            if (List.ContainsKey(systemKeyFolder))
            {
                return ListFileMapPath[systemKeyFolder];
            }
            return new List<string>();
        }
        public List<BackUpData> GetBackUpList(string systemKeyFolder)
        {
            if (List.ContainsKey(systemKeyFolder))
            {
                return List[systemKeyFolder];
            }
            return new List<BackUpData>();
        }
        public void DeleteBackUpFile(string fileMapPath)
        {
            if (File.Exists(fileMapPath))
            {
                File.Delete(fileMapPath);
            }
        }
        public void DeleteAllBackUpFiles()
        {
            foreach (var moduleFolder in SystemKeyFolderList)
            {
                foreach (var BackUpData in GetBackUpList(moduleFolder))
                {
                    File.Delete(BackUpData.FileMapPath);
                }
            }
        }

        public Dictionary<string, List<string>> ListFileMapPath { get; set; }        
        public Dictionary<string, List<BackUpData>> List { get; set; }
        public List<string> SystemKeyFolderList { get; set; }
        public string BackupRootFolder { get; set; }
        public string BackupRootFolderMapPath { get; set; }
    }
}
