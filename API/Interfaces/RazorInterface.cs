using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using Simplisity;
using DNNrocketAPI.Components;
using HandlebarsDotNet;

namespace DNNrocketAPI
{
    public interface IRazorInterface
    {
        string RenderToken(string interfaceKey, string cmd, SimplisityRazor model);
    }
    public sealed class RazorInterface
    {
        private static Dictionary<string, IRazorInterface> _instances;
        private static object _lock = new object();
        public static IRazorInterface GetInstance(string assembly, string nameSpaceClass)
        {
            var provKey = assembly + "," + nameSpaceClass;
            lock (_lock)
            {
                if ((_instances == null))
                {
                    _instances = new Dictionary<string, IRazorInterface>();
                }
                if (!_instances.ContainsKey(provKey))
                {
                    _instances.Add(provKey, CreateProvider(assembly, nameSpaceClass));
                }
            }
            return _instances[provKey];
        }
        private static IRazorInterface CreateProvider(string assembly, string nameSpaceClass)
        {
            if (!string.IsNullOrEmpty(assembly) && !string.IsNullOrEmpty(nameSpaceClass))
            {
                try
                {
                    var handle = Activator.CreateInstance(assembly.Trim(), nameSpaceClass.Trim());
                    return (IRazorInterface)handle.Unwrap();
                }
                catch (Exception ex)
                {
                    LogUtils.LogException(ex);
                    return null;
                }
            }
            return null;
        }
        private RazorInterface()
        {
        }
    }

}

