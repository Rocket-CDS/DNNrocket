using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RocketCatalog.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace DNNrocket.Category
{
    public class StartConnect : DNNrocketAPI.APInterface
    {
        private string _EntityTypeCode;
        private SystemLimpet _systemData;
        private SimplisityInfo _postInfo;
        private SimplisityInfo _paramInfo;
        private RocketInterface _rocketInterface;
        private Dictionary<string, string> _passSettings;
        private string _editLang;
        private string _currentLang;
        private string _nextLang;

        public override Dictionary<string, object> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            paramCmd = InitCmd(paramCmd, systemInfo, interfaceInfo, postInfo, paramInfo, langRequired);

            var rtnDic = new Dictionary<string, object>();

            var strOut = "";

                switch (paramCmd)
                {
                    case "categoryadmin_editlist":
                        break;
                    case "categoryadmin_edit":
                        break;
                    case "category_savelist":
                        break;
                    case "categoryadmin_delete":
                        break;
                    case "categoryadmin_save":
                        break;
                    case "categoryadmin_addimage":
                        break;

                case "catalogadmin_login":

                    break;
                    
                }

            // -----------------------------------------------------------------------
            // if we have changed language, reset the editlang.  The _nextLang is defined on the "InitCmd" function.
            if (_nextLang != _editLang) DNNrocketUtils.SetEditCulture(_nextLang);
            // -----------------------------------------------------------------------

            rtnDic.Add("outputhtml", strOut);
            return rtnDic;
        }

        public string InitCmd(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            _postInfo = postInfo;
            _paramInfo = paramInfo;

            _currentLang = DNNrocketUtils.GetCurrentCulture();
            // -----------------------------------------------------------------------
            // Change of language. 
            // _nextlang is used for returning data. 
            // _editlang is used to save the data and reset to _nextLang at end of processing in "ProcessCommand" method.
            _editLang = DNNrocketUtils.GetEditCulture(); // set editlang from url param or cookie
            _nextLang = _paramInfo.GetXmlProperty("genxml/hidden/nextlang");
            if (_nextLang == "") _nextLang = _editLang; // default to editLang
            DNNrocketUtils.SetNextCulture(_nextLang); // set the next langauge to a cookie, so the "EditFlag" razor token works.
            // -----------------------------------------------------------------------

            _systemData = new SystemLimpet(systemInfo);
            _rocketInterface = new RocketInterface(interfaceInfo);

            _passSettings = new Dictionary<string, string>();

            var securityData = new SecurityLimet(PortalUtils.GetCurrentPortalId(), _systemData.SystemKey, _rocketInterface, -1, -1);
            paramCmd = securityData.HasSecurityAccess(paramCmd, "catalogadmin_login");

            return paramCmd;
        }


    }
}
