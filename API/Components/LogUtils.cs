using DotNetNuke.Services.Exceptions;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Components
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
        public static void LogSystemClear(int daysToKeep)
        {
            var mappath = DNNrocketUtils.MapPath("Portals/_default/RocketLogs");
            if (!Directory.Exists(mappath)) Directory.CreateDirectory(mappath);
            DirectoryInfo di = new DirectoryInfo(mappath);
            foreach (FileInfo file in di.GetFiles())
            {
                if (file.CreationTime < DateTime.Now.AddDays(daysToKeep * -1)) file.Delete();
            }
        }

        //  --------------------- Debug Log files ------------------------------
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
        public static void LogTrackingClear(int portalid, int daysToKeep)
        {
            var mappath = PortalUtils.HomeDNNrocketDirectoryMapPath(portalid).TrimEnd('\\') + "\\logs";
            if (!Directory.Exists(mappath)) Directory.CreateDirectory(mappath);
            DirectoryInfo di = new DirectoryInfo(mappath);
            foreach (FileInfo file in di.GetFiles())
            {
                if (file.CreationTime < DateTime.Now.AddDays(daysToKeep)) file.Delete();
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

        //  --------------------- Private utils ------------------------------
        private static void Log(string message, string folderName)
        {
            var mappath = PortalUtils.TempDirectoryMapPath().TrimEnd('\\') + "\\" + folderName;
            if (!Directory.Exists(mappath)) Directory.CreateDirectory(mappath);
            FileUtils.AppendToLog(mappath, "debug", message);
        }
        private static void LogClear(string folderName, int daysToKeep)
        {
            LogClear(PortalUtils.GetCurrentPortalId(), folderName, daysToKeep);
        }
        private static void LogClear(int portalId, string folderName, int daysToKeep)
        {
            var mappath = PortalUtils.TempDirectoryMapPath(portalId).TrimEnd('\\') + "\\" + folderName;
            if (!Directory.Exists(mappath)) Directory.CreateDirectory(mappath);
            DirectoryInfo di = new DirectoryInfo(mappath);
            foreach (FileInfo file in di.GetFiles())
            {
                if (file.CreationTime < DateTime.Now.AddDays(daysToKeep)) file.Delete();
            }
        }

    }
}
