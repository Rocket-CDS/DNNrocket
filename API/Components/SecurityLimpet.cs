using DNNrocketAPI.Components;
using Simplisity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace DNNrocketAPI.Components
{
    /// <summary>
    /// The SecurityLimet deal with the security in the RocketEcommerce system.
    /// It takes information from the system data and the system defaults and decides if the user has security and display access
    /// </summary>
    public class SecurityLimpet
    {
        private int _userId;
        private int _tabid;
        private int _moduleid;
        private string _systemKey;
        private RocketInterface _rocketInterface;
        static ConcurrentDictionary<string, bool> _commandSecurity;  // thread safe dictionary.
        private string _defaultFileRelPath;
        private SimplisityInfo _info;
        private const RegexOptions RxOptions = RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled;
        
        private static readonly Regex[] RxListStrings = new[]
        {
            new Regex("<script[^>]*>.*?</script[^><]*>", RxOptions),
            new Regex("<script", RxOptions),
            new Regex("<input[^>]*>.*?</input[^><]*>", RxOptions),
            new Regex("<object[^>]*>.*?</object[^><]*>", RxOptions),
            new Regex("<embed[^>]*>.*?</embed[^><]*>", RxOptions),
            new Regex("<applet[^>]*>.*?</applet[^><]*>", RxOptions),
            new Regex("<form[^>]*>.*?</form[^><]*>", RxOptions),
            new Regex("<option[^>]*>.*?</option[^><]*>", RxOptions),
            new Regex("<select[^>]*>.*?</select[^><]*>", RxOptions),
            new Regex("<source[^>]*>.*?</source[^><]*>", RxOptions),
            new Regex("<iframe[^>]*>.*?</iframe[^><]*>", RxOptions),
            new Regex("<iframe.*?<", RxOptions),
            new Regex("<iframe.*?", RxOptions),
            new Regex("<ilayer[^>]*>.*?</ilayer[^><]*>", RxOptions),
            new Regex("<form[^>]*>", RxOptions),
            new Regex("</form[^><]*>", RxOptions),
            new Regex("\bonerror\b", RxOptions),
            new Regex("\bonload\b", RxOptions),
            new Regex("\bonfocus\b", RxOptions),
            new Regex("\bonblur\b", RxOptions),
            new Regex("\bonclick\b", RxOptions),
            new Regex("\bondblclick\b", RxOptions),
            new Regex("\bonchange\b", RxOptions),
            new Regex("\bonselect\b", RxOptions),
            new Regex("\bonsubmit\b", RxOptions),
            new Regex("\bonreset\b", RxOptions),
            new Regex("\bonkeydown\b", RxOptions),
            new Regex("\bonkeyup\b", RxOptions),
            new Regex("\bonkeypress\b", RxOptions),
            new Regex("\bonmousedown\b", RxOptions),
            new Regex("\bonmousemove\b", RxOptions),
            new Regex("\bonmouseout\b", RxOptions),
            new Regex("\bonmouseover\b", RxOptions),
            new Regex("\bonmouseup\b", RxOptions),
            new Regex("\bonreadystatechange\b", RxOptions),
            new Regex("\bonfinish\b", RxOptions),
            new Regex("javascript:", RxOptions),
            new Regex("vbscript:", RxOptions),
            new Regex("unescape", RxOptions),
            new Regex("alert[\\s(&nbsp;)]*\\([\\s(&nbsp;)]*'?[\\s(&nbsp;)]*[\"(&quot;)]?", RxOptions),
            new Regex(@"eval*.\(", RxOptions),
        };


        public SecurityLimpet(int portalId, string systemKey, RocketInterface rocketInterface, int tabid, int moduleid, string wrapperSystemKey = "")
        {
            try
            {
                // Check for Wrapper System for systemdefaults/comands.
                if (wrapperSystemKey != "")
                {
                    _systemKey = wrapperSystemKey;
                    SystemData = new SystemLimpet(_systemKey);
                    _defaultFileRelPath = SystemData.SystemRelPath.TrimEnd('/') + "/Installation/SystemDefaults.rules";
                    if (!File.Exists(DNNrocketUtils.MapPath(_defaultFileRelPath)))
                    {
                        _systemKey = systemKey;
                        SystemData = new SystemLimpet(_systemKey);
                    }
                }
                else
                {
                    _systemKey = systemKey;
                    SystemData = new SystemLimpet(_systemKey);
                }
                _defaultFileRelPath = SystemData.SystemRelPath.TrimEnd('/') + "/Installation/SystemDefaults.rules";
                var filenamepath = DNNrocketUtils.MapPath(_defaultFileRelPath);


                PortalId = portalId;

                _userId = UserUtils.GetCurrentUserId();
                _moduleid = moduleid;
                _tabid = tabid;
                _rocketInterface = rocketInterface;
                ValidateUser();
                _info = (SimplisityInfo)CacheUtils.GetCache(_defaultFileRelPath);
                if (_info == null)
                {
                    var xmlString = FileUtils.ReadFile(filenamepath);
                    _info = new SimplisityInfo();
                    _info.XMLData = xmlString;

                    // check for plugin commands
                    var pluginList = SystemData.GetInterfaceList();
                    foreach (var p in pluginList)
                    {
                        var pluginFileRelPath = p.TemplateRelPath.TrimEnd('/') + "/Installation/SystemDefaults.rules";
                        if (pluginFileRelPath != _defaultFileRelPath)
                        {
                            var pluginfilenamepath = DNNrocketUtils.MapPath(pluginFileRelPath);
                            var xmlString2 = FileUtils.ReadFile(pluginfilenamepath);
                            var sRec = new SimplisityRecord();
                            sRec.XMLData = xmlString2;
                            var cmdNodeList2 = sRec.XMLDoc.SelectNodes("root/commands/command");
                            foreach (XmlNode nod in cmdNodeList2)
                            {
                                _info.AddXmlNode(nod.OuterXml, "command","root/commands") ;
                            }
                        }

                    }

                    CacheUtils.SetCache(_defaultFileRelPath, _info);
               }
                _commandSecurity = (ConcurrentDictionary<string, bool>)CacheUtils.GetCache(_systemKey + "Security" + _userId);
                if (_commandSecurity == null)
                {
                    _commandSecurity = new ConcurrentDictionary<string, bool>();
                    var cmdNodeList = _info.XMLDoc.SelectNodes("root/commands/command");
                    foreach (XmlNode nod in cmdNodeList)
                    {
                        AddCommand(nod.SelectSingleNode("cmd").InnerText, Convert.ToBoolean(nod.SelectSingleNode("action").InnerText));
                    }
                    CacheUtils.SetCache(_systemKey + "Security" + _userId, _commandSecurity);
                }
            }
            catch (Exception ex)
            {
                LogUtils.LogException(ex);
            }
        }

        public SimplisityInfo Info { get { return _info; } }
        public SystemLimpet SystemData { get; set; }

        /// <summary>
        /// Checks security access for the user. logs any security errors in the debug file.
        /// </summary>
        /// <param name="paramCmd">command to be checked</param>
        /// <returns>valid command, login cmd or invalid cmd.</returns>
        public string HasSecurityAccess(string paramCmd, string loginCommand)
        {
            if (HasCommandSecurityAccess(paramCmd))
            {
                return paramCmd;
            }
            else
            {
                if (ValidCommand(paramCmd) && !HasCommandSecurityAccess(paramCmd))
                    return loginCommand;
                else
                {
                    LogUtils.LogTracking("INVALID CMD: " + paramCmd, SystemData.SystemKey);
                    return "";
                }
            }
        }
        public bool HasSecurityAccess(string paramCmd)
        {
            if (HasCommandSecurityAccess(paramCmd)) return true;
            return false;
        }
        public void AddCommand(string commandKey, bool requiresSecurity)
        {
            if (_commandSecurity.ContainsKey(commandKey)) RemoveCommand(commandKey);
            if (!_commandSecurity.ContainsKey(commandKey))
            {
                _commandSecurity.AddOrUpdate(commandKey, requiresSecurity, (key, existingVal) => { return existingVal; });
            }
            CommandCount = _commandSecurity.Count;
        }

        public void RemoveCommand(string commandKey)
        {
            bool v;
            if (!_commandSecurity.TryRemove(commandKey, out v))
            {
                // should not fail, but ignore if does.
            }

        }

        public bool ValidCommand(string commandKey)
        {
            return _commandSecurity.ContainsKey(commandKey);
        }

        public string DefaultCommandInMenu()
        {
            foreach (var c in _commandSecurity)
            {
                if (c.Value) return c.Key;
            }
            return _rocketInterface.DefaultCommand;
        }
        public bool HasCommandSecurityAccess(string commandKey)
        {
            // if the command is NOT defined, the do not give access.  Commands MUST be defined.
            if (!_commandSecurity.ContainsKey(commandKey))
            {
                return false;
            }
            if (UserUtils.IsSuperUser()) return true;
            if (ModuleUtils.HasModuleEditRights(_tabid, _moduleid) && _rocketInterface.SecurityCheckUser(PortalId, _userId))
            {
                return true;
            }
            else
            {
                if (!_commandSecurity[commandKey]) return true;  //Command defined to have no security 
                return false;
            }
        }

        public bool SecureCommand(string commandKey)
        {
            if (!_commandSecurity.ContainsKey(commandKey)) return true;
            return _commandSecurity[commandKey];
        }
        public void AddCommandList(string commandKeyCSV, bool requiresSecurity)
        {
            var clist = commandKeyCSV.Split(',');
            foreach (var c in clist)
            {
                AddCommand(c, requiresSecurity);
            }

        }
        private void ValidateUser()
        {
            if (UserUtils.IsValidUser(PortalId, _userId))
            {
                ValidUser = true;
            }
            else
            {
                ValidUser = false;
                _userId = -1;
            }
        }
        public string Get(string xpath)
        {
            return _info.GetXmlProperty(xpath);
        }

        //defaults
        public Dictionary<string, string> ArticleOrderBy()
        {
            var rtn = new Dictionary<string, string>();
            var nodList = _info.XMLDoc.SelectNodes("root/sqlorderby/article/*");
            if (nodList != null)
            {
                foreach (XmlNode nod in nodList)
                {
                    rtn.Add("sqlorderby-article-" + nod.Name, nod.InnerText);
                }
            }
            return rtn;
        }
        public Dictionary<string, string> PortalCatalogLinks()
        {
            var rtn = new Dictionary<string, string>();
            var nodList = _info.XMLDoc.SelectNodes("root/pageslinks/*");
            if (nodList != null)
            {
                foreach (XmlNode nod in nodList)
                {
                    rtn.Add(nod.Name, nod.InnerText);
                }
            }
            return rtn;
        }



        public int CommandCount { set; get; }
        public int TabId { get { return _tabid; } }
        public int ModuleId { get { return _moduleid; } }
        public bool ValidUser { get; private set; }
        public int UserId { get { return _userId; } }
        public int PortalId { get; private set; }

    }
}