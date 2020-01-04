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
            ModuleFolderList = new List<string>();

            var l = Directory.GetDirectories(BackupRootFolderMapPath);
            foreach (var d in l)
            {
                string fullPath = Path.GetFullPath(d).TrimEnd(Path.DirectorySeparatorChar);
                string projectName = Path.GetFileName(fullPath);

                var fileList = new List<BackUpData>();
                ModuleFolderList.Add(projectName);
                var l2 = Directory.GetFiles(d);
                foreach (var f in l2)
                {
                    var BackUpData = new BackUpData(f);
                    fileList.Add(BackUpData);
                }
                List.Add(projectName, fileList);
            }
        }

        public List<BackUpData> GetBackUpList(string moduleFolder)
        {
            if (List.ContainsKey(moduleFolder))
            {
                return List[moduleFolder];
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
            foreach (var moduleFolder in ModuleFolderList)
            {
                foreach (var BackUpData in GetBackUpList(moduleFolder))
                {
                    File.Delete(BackUpData.FileMapPath);
                }
            }
        }

        public Dictionary<string, List<BackUpData>> List { get; set; }
        public List<string> ModuleFolderList { get; set; }
        public string BackupRootFolder { get; set; }
        public string BackupRootFolderMapPath { get; set; }
    }
}
