using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Simplisity;
using RazorEngine.Templating;
using RazorEngine.Configuration;
using RazorEngine;
using System.Security.Cryptography;
using DotNetNuke.Entities.Users;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Security;
using System.Xml;
using DotNetNuke.Services.Localization;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Entities.Modules;
using System.Net;
using System.IO;
using DotNetNuke.Common.Lists;
using DotNetNuke.Security.Membership;
using DotNetNuke.Entities.Users.Membership;
using System.Globalization;

namespace DNNrocketAPI
{
    public static class UserUtils
    {
        public static int GetCurrentUserId()
        {
            if (UserController.Instance.GetCurrentUserInfo() != null)
            {
                return UserController.Instance.GetCurrentUserInfo().UserID;
            }
            return -1;
        }

        public static string GetCurrentUserName()
        {
            if (UserController.Instance.GetCurrentUserInfo() != null)
            {
                return UserController.Instance.GetCurrentUserInfo().Username;
            }
            return "";
        }

        public static bool GetCurrentUserIsInRole(string role)
        {
            if (UserController.Instance.GetCurrentUserInfo() != null)
            {
                return UserController.Instance.GetCurrentUserInfo().IsInRole(role);
            }
            return false;
        }

        public static List<string> GetCurrentUserRoles()
        {
            var rtnList = new List<string>();
            var u = UserController.Instance.GetCurrentUserInfo();
            foreach (var r in u.Roles)
            {
                rtnList.Add(r);
            }
            return rtnList;
        }


        public static string ResetPass(SimplisityInfo sInfo)
        {

            if (MembershipProviderConfig.PasswordRetrievalEnabled || MembershipProviderConfig.PasswordResetEnabled)
            {
                var emailaddress = sInfo.GetXmlProperty("genxml/text/emailaddress");
                var objUser = UserController.GetUserByEmail(PortalSettings.Current.PortalId, emailaddress);
                if (objUser != null)
                {

                    var settings = new MembershipPasswordSettings(objUser.PortalID);
                    var expiry = DateTime.Now.AddMinutes(settings.ResetLinkValidity);
                    if (objUser.PasswordResetExpiration < DateTime.Now)
                    {
                        objUser.PasswordResetExpiration = expiry;
                        objUser.PasswordResetToken = Guid.NewGuid();
                        UserController.UpdateUser(objUser.PortalID, objUser);
                    }
                    else if (objUser.PasswordResetExpiration > expiry)
                    {
                        objUser.PasswordResetExpiration = expiry;
                        UserController.UpdateUser(objUser.PortalID, objUser);
                    }

                    var portalSettings = PortalController.Instance.GetCurrentPortalSettings();
                    //  Mail.SendMail(objUser, MessageType.PasswordReminder, portalSettings);

                    // ************* Send email causes error in DNN ***************

                }
            }
            return "";

        }

        public static string DoLogin(SimplisityInfo systemInfo, SimplisityInfo sInfo, string userHostAddress)
        {
            var strOut = "";
            var username = sInfo.GetXmlProperty("genxml/text/username");
            var password = sInfo.GetXmlProperty("genxml/text/password");
            var rememberme = sInfo.GetXmlPropertyBool("genxml/checkbox/rememberme");

            var rtnInfo = new SimplisityInfo();


            UserLoginStatus loginStatus = new UserLoginStatus();

            rtnInfo.SetXmlProperty("genxml/loginstatus", "fail");

            UserInfo objUser;
            if (GeneralUtils.IsEmail(username))
            {
                objUser = UserController.GetUserByEmail(PortalSettings.Current.PortalId, username);
            }
            else
            {
                objUser = UserController.GetUserByName(PortalSettings.Current.PortalId, username);
            }
            if (objUser != null)
            {
                var userValid = UserController.ValidateUser(objUser, PortalSettings.Current.PortalId, false);
                if (userValid == UserValidStatus.VALID)
                {
                    UserController.UserLogin(PortalSettings.Current.PortalId, objUser, PortalSettings.Current.PortalName, userHostAddress, rememberme);
                    if (loginStatus != UserLoginStatus.LOGIN_SUCCESS || loginStatus != UserLoginStatus.LOGIN_SUPERUSER)
                    {
                        rtnInfo.SetXmlProperty("genxml/loginstatus", "ok");
                    }
                }
            }
            strOut = LoginForm(systemInfo, rtnInfo, "login", objUser.UserID);

            return strOut;
        }

        public static string LoginForm(SimplisityInfo systemInfo, SimplisityInfo sInfo, string interfacekey, int userid)
        {
            if (systemInfo != null)
            {
                // clear cookie for cmd.  This could cause a fail login loop.
                // A module MUST always have a tabid and a valid users.  Invalid cookies without tabid could cause a loop.
                DNNrocketUtils.DeleteCookieValue("s-cmd-menu-" + systemInfo.GUIDKey);
                DNNrocketUtils.DeleteCookieValue("s-fields-menu-" + systemInfo.GUIDKey);
            }

            if (userid > 0)
            {
                sInfo.SetXmlProperty("genxml/securityaccess", "You do not have security access");
            }
            var razorTempl = DNNrocketUtils.GetRazorTemplateData("LoginForm.cshtml", "/DesktopModules/DNNrocket/API", "config-w3", DNNrocketUtils.GetCurrentCulture());
            sInfo.SetXmlProperty("genxml/interfacekey", interfacekey); // make sure the login form has the correct interface command.
            return DNNrocketUtils.RazorDetail(razorTempl, sInfo);
        }

        public static string DoRegister(SimplisityInfo sInfo, string currentCulture = "")
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("fr-FR");

            var username = sInfo.GetXmlProperty("genxml/text/username");
            var useremail = sInfo.GetXmlProperty("genxml/text/useremail");
            var password = sInfo.GetXmlProperty("genxml/text/password");

            UserInfo objUser;
                objUser = new UserInfo
                {
                    PortalID = DNNrocketUtils.GetPortalId(),
                    UserID = Null.NullInteger,
                    Username = username,
                    Email = useremail,
                    FirstName = username, // accessing this and others requires Profile property access
                    LastName = string.Empty
                };
                objUser.Membership.Password = password;
                objUser.Membership.Approved = true;
                var registerstatus = UserController.CreateUser(ref objUser);
            return UserController.GetUserCreateStatus(registerstatus);
        }


    }
}
