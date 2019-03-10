using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
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

        public CommandSecurity(DNNrocketInterface interfaceInfo)
        {
            _interfaceInfo = interfaceInfo;
            _userInfo = UserController.Instance.GetCurrentUserInfo();
            ValidateUser();
        }

        public CommandSecurity(int portalid, int userid, DNNrocketInterface interfaceInfo)
        {
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
            if (_commandSecurity.ContainsKey(commandKey) && (SecurityCheckUser() || !_commandSecurity[commandKey]))
            {
                return true;
            }
            return false;
        }


    }
}
