using DotNetNuke.Instrumentation;
using DotNetNuke.Services.EventQueue;
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
            var globalSettings = new SystemGlobalData();
            if (globalSettings.Log)
            {
                ILog tracelLogger = LoggerSource.Instance.GetLogger("DNN.Trace");
                tracelLogger.Info($"ROCKET: " + message);
            }
        }
        public static void LogSystemClear(int daysToKeep)
        {
            if (daysToKeep > 0)
            {
                LogUtils.LogSystem("LogSystemClear: Keep " + daysToKeep + " days");

                var mappath = DNNrocketUtils.MapPath("Portals/_default/Logs");
                if (!Directory.Exists(mappath)) Directory.CreateDirectory(mappath);
                DirectoryInfo di = new DirectoryInfo(mappath);
                foreach (FileInfo file in di.GetFiles())
                {
                    if (file.CreationTime < DateTime.Now.AddDays(daysToKeep * -1)) file.Delete();
                }
            }
        }
        public static void LogError(string message)
        {
            ILog errorLogger = LoggerSource.Instance.GetLogger("DNN.Errors");
            errorLogger.Info($"ROCKET: " + message);
        }

        //  --------------------- Debug Log files ------------------------------
        /// <summary>
        /// Write a data file, to the Portal \DNNrocketTemp\debug folder.
        /// </summary>
        /// <param name="outFileName">Name of file</param>
        /// <param name="content">content of file</param>
        public static void OutputDebugFile(string outFileName, string content, int portalid = -1)
        {
            var mappath = PortalUtils.TempDirectoryMapPath(portalid).TrimEnd('\\') + "\\debug";
            if (!Directory.Exists(mappath)) Directory.CreateDirectory(mappath);
            FileUtils.SaveFile(mappath + "\\" + outFileName, content);
        }
        /// <summary>
        /// Used to log any actions that we may need to refer to later.  To prove what has happen.
        /// </summary>
        /// <param name="message"></param>
        [Obsolete("Use LogSystem() instead")]
        public static void LogTracking(string message, string systemkey)
        {
            LogSystem(systemkey + " - portalid:" + PortalUtils.GetCurrentPortalId() + " - " + message);
        }
        [Obsolete("Use LogSystemClear() instead")]
        public static void LogTrackingClear(int portalid, int daysToKeep)
        {
            LogSystemClear(daysToKeep);
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
