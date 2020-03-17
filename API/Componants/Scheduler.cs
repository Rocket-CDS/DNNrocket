using Simplisity;
using System;
using System.IO;
using System.Runtime.Remoting;

namespace DNNrocketAPI
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
                var systemDataList = new SystemDataList();
                foreach (var sInfo in systemDataList.GetSystemList())
                {
                    var systemData = new SystemData(sInfo.ItemID);
                    foreach (var rocketInterface in systemData.SchedulerList)
                    {
                        var cacheKey = rocketInterface.Assembly + "," + rocketInterface.NameSpaceClass;
                        var ajaxprov = (SchedulerInterface)CacheUtils.GetCache(cacheKey);
                        if (ajaxprov == null)
                        {
                            ajaxprov = SchedulerInterface.Instance(rocketInterface.Assembly, rocketInterface.NameSpaceClass);
                            CacheUtils.SetCache(cacheKey, ajaxprov);
                        }
                        ajaxprov.DoWork(systemData, rocketInterface);
                    }
                }

                this.ScheduleHistoryItem.Succeeded = true;

            }
            catch (Exception Ex)
            {
                //--intimate the schedule mechanism to write log note in schedule history
                this.ScheduleHistoryItem.Succeeded = false;
                this.ScheduleHistoryItem.AddLogNote(" Service Failed. Error:" + Ex.ToString());
                this.Errored(ref Ex);
            }
        }


    }


}
