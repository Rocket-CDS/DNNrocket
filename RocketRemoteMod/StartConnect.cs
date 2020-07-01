using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Simplisity;
using System.Collections.Generic;

namespace RocketRemoteMod
{
    public class StartConnect : DNNrocketAPI.APInterface
    {
        private SimplisityInfo _postInfo;
        private SimplisityInfo _paramInfo;
        private CommandSecurity _commandSecurity;
        private DNNrocketInterface _rocketInterface;
        private SystemData _systemData;
        private Dictionary<string, string> _passSettings;
        private string _editLang;

        public override Dictionary<string, object> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            var strOut = ""; // return nothing if not matching commands.

            paramCmd = InitCmd(paramCmd, systemInfo, interfaceInfo, postInfo, paramInfo, langRequired);

            var rtnDic = new Dictionary<string, object>();

            switch (paramCmd)
            {
                case "rocketserver_test":
                    strOut = "TEST";
                    break;
            }

            rtnDic.Add("outputhtml", strOut);
            return rtnDic;


        }

        public string InitCmd(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            _systemData = new SystemData(systemInfo);
            _rocketInterface = new DNNrocketInterface(interfaceInfo);

            _postInfo = postInfo;
            // set editlang from url param or cache
            _editLang = DNNrocketUtils.GetEditCulture();

            _paramInfo = paramInfo;

            _editLang = DNNrocketUtils.GetEditCulture();

            return paramCmd;
        }


    }

}
