using DNNrocketAPI.Components;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocketPortal.Components
{
    public static class LocalUtils
    {
        public const string ControlPath = "/DesktopModules/DNNrocket/RocketPortal";
        public const string ResourcePath = "/DesktopModules/DNNrocket/RocketPortal/App_LocalResources";

        /// <summary>
        /// Get a resouerce string from a resx file in "/DesktopModules/DNNrocketModules/RocketEcommerce/App_LocalResources" 
        /// </summary>
        /// <param name="resourceKey">[filename].[resourcekey]</param>
        /// <param name="resourceExt">[resource key extention]</param>
        /// <param name="cultureCode">[culturecode to fetch]</param>
        /// <returns></returns>
        public static string ResourceKey(string resourceKey, string resourceExt = "Text", string cultureCode = "")
        {
            return DNNrocketUtils.GetResourceString("/DesktopModules/DNNrocket/RocketPortal/App_LocalResources", resourceKey, resourceExt, cultureCode);
        }
        
        public static string RunActionProvider(PortalLimpet portalData, SimplisityInfo postInfo)
        {
            try
            {
                var assembly = postInfo.GetXmlProperty("genxml/actionassembly");
                var namespaceclass = postInfo.GetXmlProperty("genxml/actionnamespaceclass");
                var actiondata = postInfo.GetXmlProperty("genxml/actiondata");

                if (assembly == "" || namespaceclass == "") 
                    return "ERROR: Invalid Assembly or Namespace";
                else
                {
                    var prov = ActionProvider.Instance(assembly, namespaceclass);
                    var rtn = prov.DoAction(portalData, actiondata);
                }
                return "OK";
            }
            catch (Exception e)
            {
                return "ERROR: " + e.ToString();
            }
        }
    }

}
