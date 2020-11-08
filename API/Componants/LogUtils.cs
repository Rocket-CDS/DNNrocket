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

        //  --------------------- System Log for sceduler or non-portal specific log ------------------------------
        public static void LogSystem(string message)
        {
            var mappath = DNNrocketUtils.MapPath("Portals/_default/RocketLogs");
            if (!Directory.Exists(mappath)) Directory.CreateDirectory(mappath);
            FileUtils.AppendToLog(mappath, "system", message);
        }

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

        [Obsolete("LogDebug(string message) is deprecated, please use LogDebug(string message, string systemkey) instead.")]
        public static void LogDebug(string message)
        {
            Log(message, "debug");
        }
        /// <summary>
        /// Use only for DEBUG. This will only be saved inthe Temporary portal folder.  Use LogTracking for normal user action logs
        /// </summary>
        /// <param name="message"></param>
        /// <param name="systemkey"></param>
        public static void LogDebug(string message, string systemkey)
        {
            var dolog = false;
            var systemData = new SystemLimpet(systemkey);
            if (systemData.Exists && systemData.DebugMode) dolog = true;
            if (dolog) Log(message, "debug");
        }

        /// <summary>
        /// Use only for DEBUG.  Use LogTracking for normal user action logs
        /// </summary>
        public static void LogDebugClear()
        {
            LogClear("debug");
        }
        /// <summary>
        /// Write a data file, to the Portal \DNNrocketTemp\debug folder.
        /// </summary>
        /// <param name="outFileName">Name of file</param>
        /// <param name="content">content of file</param>
        public static void OutputDebugFile(string outFileName, string content)
        {
            var mappath = PortalUtils.TempDirectoryMapPath().TrimEnd('\\') + "\\debug";
            if (!Directory.Exists(mappath)) Directory.CreateDirectory(mappath);
            FileUtils.SaveFile(mappath + "\\" + outFileName, content);
        }
        /// <summary>
        /// Used to log any actions that we may need to refer to later.  To prove what has happen.
        /// </summary>
        /// <param name="message"></param>
        public static void LogTracking(string message, string systemkey)
        {
            if (systemkey == "") systemkey = "systemapi";
            var dolog = false;
            var systemData = new SystemLimpet(systemkey);
            if (systemData.Exists && systemData.LogOn) dolog = true;
            if (dolog)
            {
                var mappath = PortalUtils.HomeDNNrocketDirectoryMapPath().TrimEnd('\\') + "\\logs";
                if (!Directory.Exists(mappath)) Directory.CreateDirectory(mappath);
                FileUtils.AppendToLog(mappath, systemkey, message);
                LogSystem(systemkey + " - portalid:" + PortalUtils.GetCurrentPortalId() + "_" + message);
            }

        }
        /// <summary>
        /// Places an exception onto the DNN audit log.
        /// </summary>
        /// <param name="exc"></param>
        /// <returns></returns>
        public static string LogException(Exception exc)
        {
            CacheUtils.ClearAllCache(); // do  not want to repeat the error;
            Exceptions.LogException(exc);
            return exc.ToString();
        }

    }
}
