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
 /// <summary>
 /// NOT USED, THE LOCK MAKES IT SLOW.
 /// </summary>
 public sealed class RazorEngineSingleton
    {
        #region Singleton stuff
        private static IRazorEngineService _instance;
        private static object _lock = new object();
        public static IRazorEngineService Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        // do razor test
                        var config = new TemplateServiceConfiguration();
                        config.Debug = false;
                        config.BaseTemplateType = typeof(RazorEngineTokens<>);
                        config.DisableTempFileLocking = true;

                        // Caching provider built but not working across AppPool restart, so it is no better than the default.
                        //config.CachingProvider = new RazorCacheProvider();

                        _instance = (IRazorEngineService)RazorEngineService.Create(config);
                    }
                    return _instance;
                }
            }
        }

        #endregion
        public RazorEngineSingleton()
        {
        }
    }
}
