using DNNrocketAPI.Components;
using Simplisity;
using System;
using System.IO;
using System.Runtime.Remoting;

namespace DNNrocketAPI.Components
{
    public class RocketScheduler : DotNetNuke.Services.Scheduling.SchedulerClient
    {
        public RocketScheduler(DotNetNuke.Services.Scheduling.ScheduleHistoryItem objScheduleHistoryItem) : base()
        {
            this.ScheduleHistoryItem = objScheduleHistoryItem;
        }

        public override void DoWork()
        {
            try
            {
                LogUtils.LogSystem("START - Schedule");

                var gloablSettings = new SystemGlobalData();
                if (gloablSettings.PreCompileRazor)
                {
                    LogUtils.LogSystem("Precompile Razor - Start");
                    var rocketThemesFolder = DNNrocketUtils.MapPath("/DesktopModules/RocketThemes");
                    RenderRazorUtils.PreCompileRazorFolder(rocketThemesFolder);
                    LogUtils.LogSystem("Precompile Razor - End");
                }

                var systemDataList = new SystemLimpetList();
                foreach (var systemData in systemDataList.GetSystemList())
                {
                    if (gloablSettings.PreCompileRazorAdmin)
                    {
                        LogUtils.LogSystem("Precompile Razor Admin - Start " + systemData.SystemKey);
                        RenderRazorUtils.PreCompileRazorFolder(systemData.SystemMapPath);
                        LogUtils.LogSystem("Precompile Razor Admin - End " + systemData.SystemKey);
                    }

                    if (!systemData.IsPlugin) // plugins should be triggered by the parent system.
                    {
                        LogUtils.LogSystem("systemData.SchedulerList - Start " + systemData.SystemKey);
                        foreach (var rocketInterface in systemData.SchedulerList)
                        {
                            if (rocketInterface.IsActive)
                            {
                                try
                                {
                                    var cacheKey = rocketInterface.Assembly + "," + rocketInterface.NameSpaceClass;
                                    var ajaxprov = (SchedulerInterface)CacheUtils.GetCache(cacheKey);
                                    if (ajaxprov == null)
                                    {
                                        ajaxprov = SchedulerInterface.Instance(rocketInterface.Assembly, rocketInterface.NameSpaceClass);
                                        CacheUtils.SetCache(cacheKey, ajaxprov);
                                    }
                                    ajaxprov.DoWork();
                                }
                                catch (Exception Ex)
                                {
                                    // report individual fail for scheduler, but continue other scheduler events
                                    this.ScheduleHistoryItem.AddLogNote(" Service Failed. Error:" + Ex.ToString());
                                    LogUtils.LogSystem(" Scheduler Failed. Error:" + Ex.ToString());
                                }
                                LogUtils.LogSystem("InterfaceKey -  " + rocketInterface.InterfaceKey);
                            }
                        }
                        LogUtils.LogSystem("systemData.SchedulerList - End " + systemData.SystemKey);
                    }
                }

                LogUtils.LogSystemClear(gloablSettings.MaxLogFiles);

                this.ScheduleHistoryItem.Succeeded = true;

                LogUtils.LogSystem("END - Schedule");

            }
            catch (Exception Ex)
            {
                //--intimate the schedule mechanism to write log note in schedule history
                this.ScheduleHistoryItem.Succeeded = false;
                this.ScheduleHistoryItem.AddLogNote(" Service Failed. Error:" + Ex.ToString());
                this.Errored(ref Ex);

                LogUtils.LogSystem(" Scheduler Failed. Error:" + Ex.ToString());
            }
        }


    }

}
