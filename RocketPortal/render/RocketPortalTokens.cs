using DNNrocketAPI.Components;
using RazorEngine.Text;
using Simplisity;

namespace RocketPortal.Components
{
    public class RocketPortalTokens<T> : DNNrocketAPI.render.DNNrocketTokens<T>
    {
        public IEncodedString IsSystemValid(int portalId, string systemKey)
        {
            var systemData = new SystemLimpet(systemKey);
            var interfacekey = "rocketsystem";
            var rocketInterface = new RocketInterface(systemData.SystemInfo, interfacekey);
            var postInfo = new SimplisityInfo();
            var paramInfo = new SimplisityInfo();
            paramInfo.SetXmlProperty("genxml/hidden/portalid", portalId.ToString());
            var returnDictionary = DNNrocketUtils.GetProviderReturn("rocketsystem_valid", systemData.SystemInfo, rocketInterface, postInfo, paramInfo, "/DesktopModules/DNNrocket/api", "");

            var rtn = "";
            if (returnDictionary.ContainsKey("outputhtml")) rtn = (string)returnDictionary["outputhtml"];

            return new RawString(rtn);
        }

    }
}
