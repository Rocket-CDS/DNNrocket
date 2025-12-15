using System.Collections.Generic;

namespace DNNrocketAPI.Components
{
    public sealed class SystemSingleton
    {
        #region Singleton stuff
        private static Dictionary<string, SystemLimpet> _instances;
        private static object _lock = new object();
        public static SystemLimpet Instance(string systemKey)
        {
            lock (_lock)
            {
                systemKey = systemKey.ToLower();
                if ((_instances == null))
                {
                    _instances = new Dictionary<string, SystemLimpet>();
                }
                if (!_instances.ContainsKey(systemKey))
                {
                    var systemData = new SystemLimpet(systemKey);
                    var baseSystemKey = systemData.SystemInfo.GetXmlProperty("genxml/basesystemkey").ToLower();
                    if (baseSystemKey != "")
                    {
                        // search for additional plugins in the base system.
                        // Plugins are only on the base system.
                        var baseSystemData = new SystemLimpet(baseSystemKey);
                        foreach (var r in baseSystemData.GetInterfaceList())
                        {
                            if (systemData.GetInterface(r.InterfaceKey) == null)
                            {
                                //LogUtils.LogSystem(" Add base interface: " + r.InterfaceKey + " Active: " + r.IsActive);
                                if (r.IsActive && (r.OnSystemKey == "" || r.OnSystemKey == systemKey)) systemData.Record.AddRecordListItem("interfacedata", r.Info);
                            }
                        }
                    }

                    _instances.Add(systemKey, systemData);
                }
            }
            return _instances[systemKey];
        }
        #endregion
        public SystemSingleton()
        {
        }
    }
}
