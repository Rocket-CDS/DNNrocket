using DNNrocketAPI.Components;
using Simplisity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
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
        private string _defaultFileMapPath;
        public SecurityLimpet(int portalId, string systemKey, RocketInterface rocketInterface, int tabid, int moduleid)
        {
            try
            {
                PortalId = portalId;
                _systemKey = systemKey;
                SystemData = new SystemLimpet(_systemKey);
                _defaultFileMapPath = SystemData.SystemRelPath.TrimEnd('/') + "/Installation/SystemDefaults.config";
                _userId = UserUtils.GetCurrentUserId();
                _moduleid = moduleid;
                _tabid = tabid;
                _rocketInterface = rocketInterface;
                ValidateUser();
                Info = (SimplisityInfo)CacheUtilsDNN.GetCache(_defaultFileMapPath);
                if (Info == null)
                {
                    var filenamepath = DNNrocketUtils.MapPath(_defaultFileMapPath);
                    var xmlString = FileUtils.ReadFile(filenamepath);
                    Info = new SimplisityInfo();
                    Info.XMLData = xmlString;
                    CacheUtilsDNN.SetCache(_defaultFileMapPath, Info);

               }
                _commandSecurity = (ConcurrentDictionary<string, bool>)CacheUtilsDNN.GetCache(_systemKey + "Security" + _userId);
                if (_commandSecurity == null)
                {
                    _commandSecurity = new ConcurrentDictionary<string, bool>();
                    var cmdNodeList = Info.XMLDoc.SelectNodes("root/commands/command");
                    foreach (XmlNode nod in cmdNodeList)
                    {
                        AddCommand(nod.SelectSingleNode("cmd").InnerText, Convert.ToBoolean(nod.SelectSingleNode("action").InnerText));
                    }
                    CacheUtilsDNN.SetCache(_systemKey + "Security" + _userId, _commandSecurity);
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
        public int CommandCount { set; get; }
        public int TabId { get { return _tabid; } }
        public int ModuleId { get { return _moduleid; } }
        public bool ValidUser { get; private set; }
        public int UserId { get { return _userId; } }
        public int PortalId { get; private set; }

    }
}