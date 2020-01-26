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
        public BackUpDataList(string backupRootFolder)
        {
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

            var l = Directory.GetDirectories(BackupRootFolderMapPath);
            foreach (var d in l)
            {
                string fullPath = Path.GetFullPath(d).TrimEnd(Path.DirectorySeparatorChar);
                string projectName = Path.GetFileName(fullPath);

                var fileList = new List<BackUpData>();
                var fileMapPathList = new List<string>();                
                SystemKeyFolderList.Add(projectName);
                var l2 = Directory.GetFiles(d).Reverse();
                foreach (var f in l2)
                {
                    var BackUpData = new BackUpData(f);
                    fileList.Add(BackUpData);
                    fileMapPathList.Add(f);
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
            return null;
        }
        public List<BackUpData> GetBackUpList(string systemKeyFolder)
        {
            if (List.ContainsKey(systemKeyFolder))
            {
                return List[systemKeyFolder];
            }
            return null;
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
