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
    public sealed class RazorEngineSingleton
    {
        #region Singleton stuff
        private static IRazorEngineService _instance;
        private static object _lock = new object();
        public static IRazorEngineService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        // do razor test
                        var config = new TemplateServiceConfiguration();
                        config.Debug = false;
                        config.BaseTemplateType = typeof(RazorEngineTokens<>);
                        _instance = (IRazorEngineService)RazorEngineService.Create(config);
                    }
                }
                return _instance;
            }
        }

        #endregion
        public RazorEngineSingleton()
        {
        }
    }
}
