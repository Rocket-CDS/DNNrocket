using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Componants
{
    public class ArchiveDataList
    {
        
        public ArchiveDataList()
        {
            LoadData();
        }

        private void LoadData()
        {
            List = new Dictionary<string, List<ArchiveData>>();
            ModuleFolderList = new List<string>();

            var l = Directory.GetDirectories(DNNrocketUtils.ArchiveDirectoryMapPath());
            foreach (var d in l)
            {
                string fullPath = Path.GetFullPath(d).TrimEnd(Path.DirectorySeparatorChar);
                string projectName = Path.GetFileName(fullPath);

                var fileList = new List<ArchiveData>();
                ModuleFolderList.Add(projectName);
                var l2 = Directory.GetFiles(d);
                foreach (var f in l2)
                {
                    var archiveData = new ArchiveData(f);
                    fileList.Add(archiveData);
                }
                List.Add(projectName, fileList);
            }
        }

        public List<ArchiveData> GetArchiveList(string moduleFolder)
        {
            if (List.ContainsKey(moduleFolder))
            {
                return List[moduleFolder];
            }
            return null;
        }

        public Dictionary<string, List<ArchiveData>> List { get; set; }
        public List<string> ModuleFolderList { get; set; }

    }
}
