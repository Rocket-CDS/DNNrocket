using System;
using System.IO;

namespace RocketUtils
{
    public static class RocketUtils
    {
        public static string GetUniqueFileName(string fileName, string folderMapPath, int idx = 1, string originalFileName = "")
        {
            if (originalFileName == "") originalFileName = fileName;

            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            if (File.Exists(folderMapPath + "\\" + fileName))
            {
                fileName = GetUniqueFileName(Path.GetFileNameWithoutExtension(originalFileName) + idx + Path.GetExtension(originalFileName), folderMapPath, idx + 1, originalFileName);
            }
            return fileName;
        }

    }
}
