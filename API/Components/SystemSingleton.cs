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
            lock (_lock)
            {
                if ((_instances == null))
                {
                    _instances = new Dictionary<string, SystemLimpet>();
                }
                if (!_instances.ContainsKey(systemKey))
                {
                    _instances.Add(systemKey, new SystemLimpet(systemKey));
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
