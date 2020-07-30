using DotNetNuke.Services.Exceptions;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Componants
{
    public class LogUtils
    {
        //  --------------------- Debug Log files ------------------------------
        public static void LogTest(string filename, string datatext)
        {
            var mappath = PortalUtils.TempDirectoryMapPath().TrimEnd('\\') + "\\test";
            if (!Directory.Exists(mappath)) Directory.CreateDirectory(mappath);

            using (StreamWriter w = File.AppendText(mappath.TrimEnd('\\') + "\\" + Path.GetFileNameWithoutExtension(filename) + ".txt")) 
            {
                w.WriteLine(datatext);
                w.WriteLine("-------------------------------");
            }
        }
        public static void LogTestClear()
        {
            var mappath = PortalUtils.TempDirectoryMapPath().TrimEnd('\\') + "\\test";
            if (!Directory.Exists(mappath)) Directory.CreateDirectory(mappath);
            System.IO.DirectoryInfo di = new DirectoryInfo(mappath);
            foreach (System.IO.FileInfo file in di.GetFiles())
            {

                file.Delete();
            }
        }

        //  --------------------- Debug Log files ------------------------------
        private static void Log(string message, string folderName)
        {
            var mappath = PortalUtils.TempDirectoryMapPath().TrimEnd('\\') + "\\" + folderName;
            if (!Directory.Exists(mappath)) Directory.CreateDirectory(mappath);
            FileUtils.AppendToLog(mappath, "debug", message);
        }
        private static void LogClear(string folderName)
        {
            var mappath = PortalUtils.TempDirectoryMapPath().TrimEnd('\\') + "\\" + folderName;
            if (!Directory.Exists(mappath)) Directory.CreateDirectory(mappath);
            System.IO.DirectoryInfo di = new DirectoryInfo(mappath);
            foreach (System.IO.FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }


        //  --------------------- Debug Log files ------------------------------
        public static void LogDebug(string message)
        {
            Log(message, "debug");
        }
        public static void LogDebugClear()
        {
            LogClear("debug");
        }
        public static void LogMessage(string message)
        {
            Log(message, "message");
        }
        public static void LogMessageClear()
        {
            LogClear("message");
        }
        /// <summary>
        /// Output a data file, if the given name to the Portal \DNNrocketTemp\debug folder.
        /// </summary>
        /// <param name="outFileName">Name of file</param>
        /// <param name="content">content of file</param>
        public static void OutputDebugFile(string outFileName, string content)
        {
            var mappath = PortalUtils.TempDirectoryMapPath().TrimEnd('\\') + "\\debug";
            if (!Directory.Exists(mappath)) Directory.CreateDirectory(mappath);
            FileUtils.SaveFile(mappath + "\\" + outFileName, content);
        }

        public static void LogTracking(string message, string logName = "Log")
        {
            var mappath = PortalUtils.HomeDNNrocketDirectoryMapPath().TrimEnd('\\') + "\\logs";
            if (!Directory.Exists(mappath)) Directory.CreateDirectory(mappath);
            FileUtils.AppendToLog(mappath, logName, message);
        }
        public static string LogException(Exception exc)
        {
            Exceptions.LogException(exc);
            return exc.ToString();
        }

    }
}
