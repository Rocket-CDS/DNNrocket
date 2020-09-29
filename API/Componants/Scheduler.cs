using DNNrocketAPI.Componants;
using Simplisity;
using System;
using System.IO;
using System.Runtime.Remoting;

namespace DNNrocketAPI.Componants
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
                var portalList = PortalUtils.GetPortals();
                foreach (var portalId in portalList)
                {
                    var systemDataList = new SystemLimpetList();
                    foreach (var sInfo in systemDataList.GetSystemList())
                    {
                        var systemData = new SystemLimpet(sInfo.ItemID);
                        systemData.PortalId = portalId;
                        foreach (var rocketInterface in systemData.SchedulerList)
                        {
                            var cacheKey = rocketInterface.Assembly + "," + rocketInterface.ProviderNameSpaceClass;
                            var ajaxprov = (SchedulerInterface)CacheUtilsDNN.GetCache(cacheKey);
                            if (ajaxprov == null)
                            {
                                ajaxprov = SchedulerInterface.Instance(rocketInterface.Assembly, rocketInterface.ProviderNameSpaceClass);
                                CacheUtilsDNN.SetCache(cacheKey, ajaxprov);
                            }
                            ajaxprov.DoWork(systemData, rocketInterface);

                        }
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
