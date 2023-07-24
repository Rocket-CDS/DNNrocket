using DNNrocketAPI.Interfaces;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Components
{
    public sealed class SystemSingleton
    {
        #region Singleton stuff
        private static Dictionary<string, SystemLimpet> _instances;
        private static object _lock = new object();
        public static SystemLimpet Instance(string systemKey)
        {
            return Instance(systemKey, systemKey);
        }
        public static SystemLimpet Instance(string systemKey, string baseSystemKey)
        {
            lock (_lock)
            {
                if (baseSystemKey == "") baseSystemKey = systemKey;
                if ((_instances == null))
                {
                    _instances = new Dictionary<string, SystemLimpet>();
                }
                if (!_instances.ContainsKey(systemKey + baseSystemKey))
                {
                    var systemData = new SystemLimpet(systemKey);
                    if (baseSystemKey != systemKey)
                    {
                        // search for additional plugins in the base system.
                        // Plugins are only on the base system.
                        var baseSystemData = new SystemLimpet(baseSystemKey);
                        foreach (var r in baseSystemData.GetInterfaceList())
                        {
                            if (systemData.GetInterface(r.InterfaceKey) == null)
                            {
                                LogUtils.LogSystem(" Add base interface: " + r.InterfaceKey);
                                systemData.Record.AddRecordListItem("interfacedata", r.Info);
                            }
                        }
                    }

                    _instances.Add(systemKey + baseSystemKey, systemData);
                }
            }
            return _instances[systemKey + baseSystemKey];
        }
        #endregion
        public SystemSingleton()
        {
        }
    }
}
