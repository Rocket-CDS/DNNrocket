using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Componants
{
    public class CommandSecurity
    {
        private int _userId;
        private UserInfo _userInfo;
        private Dictionary<string, bool> _commandSecurity;
        private DNNrocketInterface _interfaceInfo;
        private int _tabid;
        private int _moduleid;

        public CommandSecurity(int tabId, int moduleId, DNNrocketInterface interfaceInfo)
        {
            _tabid = tabId;
            _moduleid = moduleId;
            _interfaceInfo = interfaceInfo;
            _userInfo = UserController.Instance.GetCurrentUserInfo();
            ValidateUser();
        }

        public CommandSecurity(int portalid, int userid, int tabId, int moduleId, DNNrocketInterface interfaceInfo)
        {
            _tabid = tabId;
            _moduleid = moduleId;
            _interfaceInfo = interfaceInfo;
            _userInfo = UserController.Instance.GetUserById(portalid, userid);
            ValidateUser();
        }

        private void ValidateUser()
        {
            _commandSecurity = new Dictionary<string, bool>(); 
            if (_userInfo != null)
            {
                ValidUser = true;
                _userId = _userInfo.UserID;
            }
            else
            {
                ValidUser = false;
                _userId = -1;
            }
        }

        public int TabId { get { return _tabid; } }
        public int ModuleId { get { return _moduleid; } }

        public bool ValidUser { get; private set; }

        public int UserId { get { return _userId; } }

        public bool SecurityCheckIsSuperUser()
        {
            if (!ValidUser) return false;
            return _userInfo.IsSuperUser;
        }

        public bool SecurityCheckUser()
        {
            if (!ValidUser) return false;
            if (SecurityCheckIsSuperUser()) return true;
            if (_interfaceInfo != null)
            {
                if (_interfaceInfo.GetXmlPropertyBool("genxml/checkboxlist/securityroles/chk[@data='Administrators']/@value"))
                {
                    if (_userInfo.IsInRole("Administrators")) return true;
                }
                if (_interfaceInfo.GetXmlPropertyBool("genxml/checkboxlist/securityroles/chk[@data='Manager']/@value"))
                {
                    if (_userInfo.IsInRole("Manager")) return true;
                }
                if (_interfaceInfo.GetXmlPropertyBool("genxml/checkboxlist/securityroles/chk[@data='Editor']/@value"))
                {
                    if (_userInfo.IsInRole("Editor")) return true;
                }
                if (_interfaceInfo.GetXmlPropertyBool("genxml/checkboxlist/securityroles/chk[@data='ClientEditor']/@value"))
                {
                    if (_userInfo.IsInRole("ClientEditor")) return true;
                }
            }

            return false;
        }

        public void SignUserOut()
        {
            var ps = new PortalSecurity();
            ps.SignOut();
        }

        public void AddCommand(string commandKey,bool requiresSecurity)
        {
            _commandSecurity.Add(commandKey, requiresSecurity);
        }

        public void RemoveCommand(string commandKey)
        {
            _commandSecurity.Remove(commandKey);
        }

        public bool ValidCommand(string commandKey)
        {
            return _commandSecurity.ContainsKey(commandKey);
        }

        public bool SecurityCommandCheck(string commandKey)
        {
            if (!_commandSecurity.ContainsKey(commandKey)) return false;  // command does not exists.
            if (!_commandSecurity[commandKey]) return true; // No security needed.
            if (_commandSecurity[commandKey] && SecurityCheckUser() && HasModuleEditRights()) return true; // passed security check
            return false; // no security match
        }

        public bool HasModuleEditRights()
        {
           try
            {
                if (_tabid == -1 && _moduleid == -1) return true;
                if (_tabid == 0) return false;
                if (_moduleid == 0) return false;
                var moduleInfo = ModuleController.Instance.GetModule(_moduleid, _tabid, false);
                if (ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "MANAGE", moduleInfo))
                {
                    return true;
                }
            }
            catch (Exception exc) 
            {
                // We may not have all infomration from DNN to do this, so just return false.
                var msg = exc.ToString();
                return false;
            }
            return false;
        }


    }
}
