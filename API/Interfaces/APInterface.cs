using DNNrocketAPI.Components;
using DotNetNuke.Web.DDRMenu;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private static IProcessCommand _instance;
        private static object _lock = new object();
        public static IProcessCommand GetInstance(string assembly, string nameSpaceClass)
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
