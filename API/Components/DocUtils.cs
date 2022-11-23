using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Simplisity;

namespace DNNrocketAPI.Components
{
    public class DocUtils
    {
        /// <summary>
        /// Only .zip and .pdf are allowed to be uploaded by any user, apart from SuperUser, who can upload anything.
        /// </summary>
        /// <param name="strExtension"></param>
        /// <returns></returns>
        public static bool IsSafeFileType(string strExtension)
        {
            return strExtension.ToLower() == ".pdf" | strExtension.ToLower() == ".zip" | UserUtils.IsSuperUser();
        }
        public static Dictionary<string,string> SecureFiles(List<string> fileList)
        {
            var rtn = new Dictionary<string, string>();
            foreach (var f in fileList)
            {
                var newFileMapPath = Path.GetDirectoryName(f) + "\\" + GeneralUtils.GetGuidKey();
                File.Copy(f, newFileMapPath);
                if (!rtn.ContainsKey(f)) rtn.Add(f, newFileMapPath);
                File.Delete(f);
            }
            return rtn;
        }
        public static List<string> UploadBase64file(string[] filenameList, string[] filebase64List, string docFolderMapPath)
        {
            var rtn = new List<string>();
            if (filebase64List.Length == filenameList.Length && filenameList.Length > 0)
            {
                var lp = 0;
                foreach (var ncode in filenameList)
                {
                    var fname = GeneralUtils.DeCode(ncode);
                    if (!ImgUtils.IsImageFile(Path.GetExtension(fname)) && IsSafeFileType(Path.GetExtension(fname)))
                    {
                        var docFileMapPath = docFolderMapPath.TrimEnd('/') + "/" + FileUtils.RemoveInvalidFileChars(fname);
                        if (File.Exists(docFileMapPath))
                        {
                            docFileMapPath = docFolderMapPath.TrimEnd('/') + "/" + FileUtils.RemoveInvalidFileChars(Path.GetFileNameWithoutExtension(fname) + GeneralUtils.GetRandomKey(4) + Path.GetExtension(fname));
                        }
                        var fbase64 = filebase64List[lp];
                        fbase64 = fbase64.Split(',')[1];
                        var bytes = Convert.FromBase64String(fbase64.Replace(" ", "+")); // string altered during call. Fix with replace.
                        using (var fileData = new FileStream(docFileMapPath, FileMode.Create))
                        {
                            fileData.Write(bytes, 0, bytes.Length);
                            fileData.Flush();
                        }
                        rtn.Add(docFileMapPath);
                        lp += 1;
                    }
                    else
                    {
                        LogUtils.LogSystem("Attempt to upload invalid document type : " + fname);
                    }
            }
            }
            return rtn;
        }
    }

   
}
