using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using Simplisity;
using DNNrocketAPI.Components;
using DNNrocketAPI.Interfaces;

namespace DNNrocketAPI
{

    public interface IEventAction
    {
        Dictionary<string, object> BeforeEvent(string paramCmd, SystemLimpet systemData, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "");
        Dictionary<string, object> AfterEvent(string paramCmd, SystemLimpet systemData, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "");
    }

    public sealed class EventInterface
    {
        private static Dictionary<string, IEventAction> _instances;
        private static object _lock = new object();
        public static IEventAction GetInstance(string assembly, string nameSpaceClass)
        {
            var provKey = assembly + "," + nameSpaceClass;
            lock (_lock)
            {
                if ((_instances == null))
                {
                    _instances = new Dictionary<string, IEventAction>();
                }
                if (!_instances.ContainsKey(provKey))
                {
                    _instances.Add(provKey, CreateProvider(assembly, nameSpaceClass));
                }
            }
            return _instances[provKey];
        }
        private static IEventAction CreateProvider(string assembly, string nameSpaceClass)
        {
            if (!string.IsNullOrEmpty(assembly) && !string.IsNullOrEmpty(nameSpaceClass))
            {
                try
                {
                    var handle = Activator.CreateInstance(assembly.Trim(), nameSpaceClass.Trim());
                    return (IEventAction)handle.Unwrap();
                }
                catch (Exception ex)
                {
                    LogUtils.LogException(ex);
                    return null;
                }
            }
            return null;
        }
        private EventInterface()
        {
        }
    }

}

