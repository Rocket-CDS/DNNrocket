using System;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Security;
using System.Xml;
using DotNetNuke.Services.Localization;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.FileSystem;
using System.IO.Compression;
using DataProvider = DotNetNuke.Data.DataProvider;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Common;
using DNNrocketAPI.Componants;
using DotNetNuke.Services.Scheduling;
using Simplisity;

namespace DNNrocketAPI.Componants
{
    public static class SchedulerUtils
    {

        public static void SchedulerInstall()
        {
            var globalData = new SystemGlobalData();
            if (!globalData.SchedulerIsInstalled)
            {

                SchedulingProvider scheduler = SchedulingProvider.Instance();

                var typeFullName = "DNNrocketAPI.Componants.RocketScheduler,DNNrocketAPI";
                var s = scheduler.GetSchedule(typeFullName, "");
                if (s == null)
                {
                    ScheduleItem scheduleItem = new ScheduleItem();

                    scheduleItem.TypeFullName = typeFullName;
                    scheduleItem.TimeLapse = 1;
                    scheduleItem.TimeLapseMeasurement = "h";
                    scheduleItem.RetryTimeLapse = 30;
                    scheduleItem.RetryTimeLapseMeasurement = "m";
                    scheduleItem.RetainHistoryNum = 10;
                    scheduleItem.AttachToEvent = "";
                    scheduleItem.CatchUpEnabled = false;
                    scheduleItem.Enabled = true;
                    scheduleItem.ObjectDependencies = "";
                    //scheduleItem.Servers = null;
                    scheduleItem.FriendlyName = "RocketSceduler";
                    //scheduleItem.ScheduleStartDate = null;

                    scheduler.AddSchedule(scheduleItem);

                    globalData.SchedulerIsInstalled = true;
                    globalData.SchedulerIsEnabled = true;
                    globalData.Update();

                    CacheUtilsDNN.ClearAllCache();
                }
            }
        }

        public static void SchedulerUnInstall()
        {
            SchedulingProvider scheduler = SchedulingProvider.Instance();

            var typeFullName = "DNNrocketAPI.Componants.RocketScheduler,DNNrocketAPI";
            var s = scheduler.GetSchedule(typeFullName, "");
            if (s != null)
            {
                scheduler.DeleteSchedule(s);

                var globalData = new SystemGlobalData();
                globalData.SchedulerIsInstalled = false;
                globalData.SchedulerIsEnabled = false;
                globalData.Update();

                CacheUtilsDNN.ClearAllCache();
            }
        }

        public static void SchedulerStatus(bool enabled)
        {
            SchedulingProvider scheduler = SchedulingProvider.Instance();

            var typeFullName = "DNNrocketAPI.Componants.RocketScheduler,DNNrocketAPI";
            var s = scheduler.GetSchedule(typeFullName, "");
            if (s != null)
            {
                s.Enabled = enabled;

                var globalData = new SystemGlobalData();
                globalData.SchedulerIsInstalled = true;
                globalData.SchedulerIsEnabled = enabled;
                globalData.Update();

                CacheUtilsDNN.ClearAllCache();
            }
        }

        public static bool SchedulerIsEnabled()
        {
            SchedulingProvider scheduler = SchedulingProvider.Instance();
            var typeFullName = "DNNrocketAPI.Componants.RocketScheduler,DNNrocketAPI";
            var globalData = new SystemGlobalData();
            var s = scheduler.GetSchedule(typeFullName, "");
            if (s != null)
            {
                globalData.SchedulerIsInstalled = true;
                globalData.SchedulerIsEnabled = s.Enabled;
                globalData.Update();
                if (s.Enabled) return true;
            }
            else
            {
                globalData.SchedulerIsInstalled = false;
                globalData.SchedulerIsEnabled = false;
                globalData.Update();
            }
            return false;
        }

        public static bool SchedulerIsInstalled()
        {
            SchedulingProvider scheduler = SchedulingProvider.Instance();
            var typeFullName = "DNNrocketAPI.Componants.RocketScheduler,DNNrocketAPI";
            var globalData = new SystemGlobalData();
            var s = scheduler.GetSchedule(typeFullName, "");
            if (s != null)
            {
                globalData.SchedulerIsInstalled = true;
                globalData.SchedulerIsEnabled = s.Enabled;
                globalData.Update();
                return true;
            }
            else
            {
                globalData.SchedulerIsInstalled = false;
                globalData.SchedulerIsEnabled = false;
                globalData.Update();
            }
            return false;
        }

    }
}
