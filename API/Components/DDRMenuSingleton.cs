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

namespace DNNrocketAPI.Components
{
    public sealed class DDRMenuSingleton
    {
        private static string _providerAssembyClass;
        private static INodeManipulator _instance;
        private static object _lock = new object();
        public static INodeManipulator GetInstance(string providerAssembyClass)
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance = CreateProvider(_providerAssembyClass);
                }
            }
            return _instance;
        }
        private static INodeManipulator CreateProvider(string providerAssembyClass)
        {
            if (!string.IsNullOrEmpty(providerAssembyClass))
            {
                var prov = providerAssembyClass.Split(Convert.ToChar(","));
                try
                {
                    var handle = Activator.CreateInstance(prov[1], prov[0]);
                    return (INodeManipulator)handle.Unwrap();
                }
                catch (Exception ex)
                {
                    // Error in provider is invalid provider, so remove from providerlist.
                    return null;
                }
            }
            return null;
        }
        private DDRMenuSingleton()
        {
        }
    }
}
