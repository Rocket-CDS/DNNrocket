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
        private static IEventAction _instance;
        private static object _lock = new object();
        public static IEventAction GetInstance(string assembly, string nameSpaceClass)
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance = CreateProvider(assembly, nameSpaceClass);
                }
            }
            return _instance;
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

