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
                var systemDataList = new SystemLimpetList();
                foreach (var systemData in systemDataList.GetSystemList())
                {
                    foreach (var rocketInterface in systemData.SchedulerList)
                    {
                        if (rocketInterface.IsActive)
                        {
                            try
                            {
                                var cacheKey = rocketInterface.Assembly + "," + rocketInterface.NameSpaceClass;
                                var ajaxprov = (SchedulerInterface)CacheUtilsDNN.GetCache(cacheKey);
                                if (ajaxprov == null)
                                {
                                    ajaxprov = SchedulerInterface.Instance(rocketInterface.Assembly, rocketInterface.NameSpaceClass);
                                    CacheUtilsDNN.SetCache(cacheKey, ajaxprov);
                                }
                                ajaxprov.DoWork();
                            }
                            catch (Exception Ex)
                            {
                                // report individual fail for scheduler, but continue other scheduler events
                                this.ScheduleHistoryItem.AddLogNote(" Service Failed. Error:" + Ex.ToString());
                                LogUtils.LogSystem(" Scheduler Failed. Error:" + Ex.ToString());
                            }

                        }
                    }
                }

                LogUtils.LogSystemClear(7);

                this.ScheduleHistoryItem.Succeeded = true;

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
