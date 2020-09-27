using DNNrocketAPI.Componants;
using Simplisity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace DNNrocketAPI.Componants
{
    /// <summary>
    /// The SecurityLimet deal with the security in the RocketEcommerce system.
    /// It takes information from the system data and the system defaults and decides if the user has security and display access
    /// </summary>
    public class SecurityLimet
    {
        private int _userId;
        private int _tabid;
        private int _moduleid;
        private string _systemKey;
        private RocketInterface _rocketInterface;
        static ConcurrentDictionary<string, bool> _commandSecurity;  // thread safe dictionary.
        private string _defaultFileMapPath;
        public SecurityLimet(int portalId, string systemKey, RocketInterface rocketInterface, int tabid, int moduleid)
        {
            try
            {
                PortalId = portalId;
                _systemKey = systemKey;
                SystemData = new SystemLimpet(_systemKey);
                _defaultFileMapPath = SystemData.SystemRelPath.TrimEnd('/') + "/Installation/SystemDefaults.xml";
                _userId = UserUtils.GetCurrentUserId();
                _moduleid = moduleid;
                _tabid = tabid;
                _rocketInterface = rocketInterface;
                ValidateUser();
                Info = (SimplisityInfo)CacheUtils.GetCache(_defaultFileMapPath);
                if (Info == null)
                {
                    var filenamepath = DNNrocketUtils.MapPath(_defaultFileMapPath);
                    var xmlString = FileUtils.ReadFile(filenamepath);
                    Info = new SimplisityInfo();
                    Info.XMLData = xmlString;
                    CacheUtils.SetCache(_defaultFileMapPath, Info);

               }
                _commandSecurity = (ConcurrentDictionary<string, bool>)CacheUtils.GetCache(_systemKey + "Security" + _userId);
                if (_commandSecurity == null)
                {
                    _commandSecurity = new ConcurrentDictionary<string, bool>();
                    var cmdNodeList = Info.XMLDoc.SelectNodes("root/commands/command");
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

        public SimplisityInfo Info { get; set; }
        public SystemLimpet SystemData { get; set; }

        /// <summary>
        /// Checks security access for the user. logs any security errors in the debug file.
        /// </summary>
        /// <param name="paramCmd">command to be checked</param>
        /// <returns>valid command, login cmd or invalid cmd.</returns>
        public string HasSecurityAccess(string paramCmd)
        {
            if (paramCmd.StartsWith("portalshops_"))
            {
                if (!UserUtils.IsSuperUser()) return "rocketecommerce_login"; // only superuser allowed to access portalshop commands.
            }

            if (HasCommandSecurityAccess(paramCmd))
            {
                return paramCmd;
            }
            else
            {
                if (ValidCommand(paramCmd))
                    return "rocketecommerce_login";
                else
                {
                    LogUtils.LogDebug("INVALID CMD: " + paramCmd, SystemData.SystemKey);
                    return "rocketecommerce_invalidcommand";
                }
            }
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
        private bool HasCommandSecurityAccess(string commandKey)
        {
            if (UserUtils.IsSuperUser()) return true;
            // if the command is NOT defined, the do not give access.  Commands MUST be defined.
            if (!_commandSecurity.ContainsKey(commandKey))
            {
                LogUtils.LogDebug("ERROR - Command NOT defined: " + commandKey, _systemKey);
                return false;
            }
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
        public int CommandCount { set; get; }
        public int TabId { get { return _tabid; } }
        public int ModuleId { get { return _moduleid; } }
        public bool ValidUser { get; private set; }
        public int UserId { get { return _userId; } }
        public int PortalId { get; private set; }

    }
}