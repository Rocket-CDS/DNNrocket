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
    public sealed class SecuritySingleton
    {
        #region Singleton stuff
        private static Dictionary<string, SecurityLimpet> _instances;
        private static object _lock = new object();
        public static SecurityLimpet Instance(int portalId, string systemKey, RocketInterface rocketInterface, int tabid = -1, int moduleid = -1, string wrapperSystemKey = "")
        {
            var cacheKey = $"{portalId}_{systemKey}_{rocketInterface}_{tabid}_{moduleid}_{wrapperSystemKey}";
            lock (_lock)
            {
                if ((_instances == null))
                {
                    _instances = new Dictionary<string, SecurityLimpet>();
                }
                if (!_instances.ContainsKey(cacheKey))
                {
                    var securityData = new SecurityLimpet(portalId, systemKey, rocketInterface, tabid, moduleid, wrapperSystemKey);
                    _instances.Add(cacheKey, securityData);
                }
            }
            return _instances[cacheKey];
        }
        #endregion
        public SecuritySingleton()
        {
        }
    }
}
