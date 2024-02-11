using DNNrocketAPI.Components;
using Simplisity;
using System;
using System.IO;
using System.Runtime.Remoting;
using System.Threading;

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
                    var rocketThemesFolder = DNNrocketUtils.MapPath("/DesktopModules/RocketThemes");
                    System.Threading.ThreadStart threadStart = delegate { RenderRazorUtils.PreCompileRazorFolder(rocketThemesFolder); };
                    System.Threading.Thread thread = new System.Threading.Thread(threadStart);
                    thread.IsBackground = true;
                    thread.Start();
                }

                // Clear testing cache for real Scheduler processing
                CacheUtils.ClearTestingCache();

                var doWork = new SchedulerDoWork();
                doWork.DoWork();

                if (gloablSettings.PreCompileRazorAdmin)
                {
                    var systemDataList = new SystemLimpetList();
                    foreach (var systemData in systemDataList.GetSystemActiveList())
                    {
                        System.Threading.ThreadStart threadStart = delegate { RenderRazorUtils.PreCompileRazorFolder(systemData.SystemMapPath); };
                        System.Threading.Thread thread = new System.Threading.Thread(threadStart);
                        thread.IsBackground = true;
                        thread.Start();
                    }
                }

                this.ScheduleHistoryItem.Succeeded = true;
                LogUtils.LogSystemClear(gloablSettings.MaxLogFiles);
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

    public class SchedulerDoWork
    {
        public SchedulerDoWork()
        {
        }
        public void DoWork()
        {
            try
            {
                var systemDataList = new SystemLimpetList();
                foreach (var systemData in systemDataList.GetSystemActiveList())
                {
                    if (!systemData.IsPlugin) // plugins should be triggered by the parent system.
                    {
                        LogUtils.LogSystem("SchedulerDoWork - START: " + systemData.SystemKey);
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
                                        LogUtils.LogSystem("Scheduler Create Instance: " + cacheKey);
                                    }
                                    ajaxprov.DoWork();
                                }
                                catch (Exception Ex)
                                {
                                    LogUtils.LogException(Ex);
                                    LogUtils.LogSystem(" Scheduler Failed. Error:" + Ex.ToString());
                                }
                                LogUtils.LogSystem("InterfaceKey -  " + rocketInterface.InterfaceKey);
                            }
                        }
                        LogUtils.LogSystem("SchedulerDoWork - END: " + systemData.SystemKey);
                    }
                }
            }
            catch (Exception Ex)
            {
                LogUtils.LogException(Ex);
                LogUtils.LogSystem("Scheduler DoWork Failed. Error:" + Ex.ToString());
            }
        }

    }

}
