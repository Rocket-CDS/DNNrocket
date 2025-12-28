using DNNrocketAPI.Components;
using DotNetNuke.Web.DDRMenu;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DNNrocketAPI.Interfaces
{
    public interface IProcessCommand
    {
        Dictionary<string, object> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "");
    }

    public sealed class APInterface
    {
        private static Dictionary<string, IProcessCommand> _instances;
        private static object _lock = new object();
        public static IProcessCommand GetInstance(string assembly, string nameSpaceClass)
        {
            //Thread safe
            // DO NOT implement a singleton for this, it needs to be thread safe.
            //LogUtils.LogSystem("START CreateProvider: " + assembly + ", " + nameSpaceClass);
            var rtn = CreateProvider(assembly, nameSpaceClass);
            //LogUtils.LogSystem("END CreateProvider: " + assembly + ", " + nameSpaceClass);
            return rtn;

            //var provKey = assembly + "," + nameSpaceClass;
            //lock (_lock)
            //{

            //    if ((_instances == null))
            //    {
            //        _instances = new Dictionary<string, IProcessCommand>();
            //    }
            //    if (!_instances.ContainsKey(provKey))
            //    {
            //        _instances.Add(provKey, CreateProvider(assembly, nameSpaceClass));
            //    }
            //}
            //return _instances[provKey];
        }
        public static int InstanceCount()
        {
            return _instances.Count;
        }
        private static IProcessCommand CreateProvider(string assembly, string nameSpaceClass)
        {
            if (!string.IsNullOrEmpty(assembly) && !string.IsNullOrEmpty(nameSpaceClass))
            {
                try
                {
                    var handle = Activator.CreateInstance(assembly.Trim(), nameSpaceClass.Trim());
                    return (IProcessCommand)handle.Unwrap();
                }
                catch (Exception ex)
                {
                    LogUtils.LogException(ex);
                    return null;
                }
            }
            return null;
        }
        private APInterface()
        {
        }
    }
}
