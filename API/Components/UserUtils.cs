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
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.Services.Mail;
using DotNetNuke.Common;
using DotNetNuke.UI.UserControls;
using DotNetNuke.Security.Roles;
using System.Collections;

namespace DNNrocketAPI.Components
{
    public static class UserUtils
    {
        public static int GetCurrentUserId()
        {
            try
            {
                if (UserController.Instance.GetCurrentUserInfo() != null)
                {
                    if (HttpContext.Current.User.Identity.IsAuthenticated)
                    {
                        return UserController.Instance.GetCurrentUserInfo().UserID;
                    }
                }
                return -1;
            }
            catch (Exception ex)
            {
                var ms = ex.ToString();
                return -1; 
            }
        }

        public static void DeleteUser(int portalId, int userId, bool notify = false, bool deleteAdmin = false)
        {
            try
            {
                if (UserController.Instance.GetCurrentUserInfo() != null)
                {
                    var userInfo = UserController.Instance.GetUserById(portalId, userId);
                    UserController.DeleteUser(ref userInfo, notify, deleteAdmin);
                }
            }
            catch (Exception ex)
            {
                LogUtils.LogException(ex);
            }

        }

        public static string GetCurrentUserName()
        {
            if (UserController.Instance.GetCurrentUserInfo() != null)
            {
                return UserController.Instance.GetCurrentUserInfo().Username;
            }
            return "";
        }
        public static string GetCurrentUserEmail()
        {
            if (UserController.Instance.GetCurrentUserInfo() != null)
            {
                return UserController.Instance.GetCurrentUserInfo().Email;
            }
            return "";
        }
        public static List<string> GetUserRoles(int portalid = -1, int userId = -1)
        {
            var rtnList = new List<string>();
            var u = UserController.Instance.GetCurrentUserInfo();
            if (portalid >= 0 && userId > 0) u = UserController.Instance.GetUserById(portalid, userId);
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
        public static bool ResetAndChangePassword(int userId, string newPassword)
        {
            var objUser = UserController.GetUserById(PortalSettings.Current.PortalId, userId);
            if (objUser != null)
            {
                return UserController.ResetAndChangePassword(objUser, newPassword);
            }
            return false;
        }

        public static string DoLogin(SimplisityInfo systemInfo, SimplisityInfo sInfo, string userHostAddress)
        {
            var strOut = "";
            var username = sInfo.GetXmlProperty("genxml/text/username");
            var password = sInfo.GetXmlProperty("genxml/hidden/password");
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
                    UserController.UserLogin(PortalSettings.Current.PortalId, objUser.Username, password, "", PortalSettings.Current.PortalName, userHostAddress, ref loginStatus, rememberme);
                    if (loginStatus == UserLoginStatus.LOGIN_SUCCESS || loginStatus == UserLoginStatus.LOGIN_SUPERUSER)
                    {
                        rtnInfo.SetXmlProperty("genxml/loginstatus", "ok");
                    }
                }
            }
            strOut = LoginForm(systemInfo, rtnInfo, "login", -1);

            return strOut;
        }
        public static string LoginForm(SimplisityInfo systemInfo, SimplisityInfo sInfo, string interfacekey, int userid)
        {
            if (userid > 0)
            {
                sInfo.SetXmlProperty("genxml/securityaccess", "You do not have security access");
            }
            var razorTempl = RenderRazorUtils.GetRazorTemplateData("LoginForm.cshtml", "/DesktopModules/DNNrocket/API", "config-w3", DNNrocketUtils.GetCurrentCulture(), "1.0", true);
            sInfo.SetXmlProperty("genxml/interfacekey", interfacekey); // make sure the login form has the correct interface command.
            return RenderRazorUtils.RazorDetail(razorTempl, sInfo);
        }

        public static string RegisterForm(SimplisityInfo systemInfo, SimplisityInfo sInfo, string interfacekey, int userid)
        {
            //if (systemInfo != null)
            //{
            //    // clear cookie for cmd.  This could cause a fail login loop.
            //    // A module MUST always have a tabid and a valid users.  Invalid cookies without tabid could cause a loop.
            //    DNNrocketUtils.DeleteCookieValue("s-cmd-menu-" + systemInfo.GUIDKey);
            //    DNNrocketUtils.DeleteCookieValue("s-fields-menu-" + systemInfo.GUIDKey);
            //}

            if (userid > 0)
            {
                sInfo.SetXmlProperty("genxml/securityaccess", "You do not have security access");
            }
            var razorTempl = RenderRazorUtils.GetRazorTemplateData("RegisterForm.cshtml", "/DesktopModules/DNNrocket/API", "config-w3", DNNrocketUtils.GetCurrentCulture(), "1.0", true);
            sInfo.SetXmlProperty("genxml/interfacekey", interfacekey); // make sure the login form has the correct interface command.
            return RenderRazorUtils.RazorDetail(razorTempl, sInfo);
        }

        public static string RegisterUser(SimplisityInfo sInfo, string currentCulture = "")
        {
            //TODO: IMPORTANT check security for this. (dcl)

            if (currentCulture == "") currentCulture = "en-US";

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(currentCulture);

            var username = sInfo.GetXmlProperty("genxml/text/username");
            var useremail = sInfo.GetXmlProperty("genxml/text/email");
            var password = sInfo.GetXmlProperty("genxml/hidden/password");
            var confirmpassword = sInfo.GetXmlProperty("genxml/hidden/confirmpassword");
            var approved = sInfo.GetXmlPropertyBool("genxml/checkbox/autorize");
            if (sInfo.GetXmlProperty("genxml/checkbox/autorize") == "") approved = true;
            var randompassword = sInfo.GetXmlPropertyBool("genxml/checkbox/randompassword");
            if (randompassword)
            {
                password = GeneralUtils.GetUniqueKey(8);
                confirmpassword = password;
            }
            var displayname = sInfo.GetXmlProperty("genxml/text/displayname");


            UserInfo objUser;
            objUser = new UserInfo
            {
                PortalID = PortalUtils.GetPortalId(),
                UserID = Null.NullInteger,
                Username = username,
                Email = useremail,
                FirstName = username, // accessing this and others requires Profile property access
                LastName = string.Empty,
                DisplayName = displayname
            };
            objUser.Membership.Password = password;
            objUser.Membership.PasswordConfirm = confirmpassword;
            objUser.Membership.Approved = approved;
            var registerstatus = UserController.CreateUser(ref objUser);

            return GetUserCreateStatus(registerstatus);
        }

        private static string GetUserCreateStatus(UserCreateStatus userRegistrationStatus)
        {
            switch (userRegistrationStatus)
            {
                case UserCreateStatus.DuplicateEmail:
                    return DNNrocketUtils.GetResourceString("/App_GlobalResources", "SharedResources.UserEmailExists");
                case UserCreateStatus.InvalidAnswer:
                    return DNNrocketUtils.GetResourceString("/App_GlobalResources", "SharedResources.InvalidAnswer");
                case UserCreateStatus.InvalidEmail:
                    return DNNrocketUtils.GetResourceString("/App_GlobalResources", "SharedResources.InvalidEmail");
                case UserCreateStatus.InvalidPassword:
                    string strInvalidPassword = DNNrocketUtils.GetResourceString("/App_GlobalResources", "SharedResources.InvalidPassword");
                    strInvalidPassword = strInvalidPassword.Replace("[PasswordLength]", MembershipProviderConfig.MinPasswordLength.ToString());
                    strInvalidPassword = strInvalidPassword.Replace("[NoneAlphabet]", MembershipProviderConfig.MinNonAlphanumericCharacters.ToString());
                    return strInvalidPassword;
                case UserCreateStatus.PasswordMismatch:
                    return DNNrocketUtils.GetResourceString("/App_GlobalResources", "SharedResources.PasswordMismatch");
                case UserCreateStatus.InvalidQuestion:
                    return DNNrocketUtils.GetResourceString("/App_GlobalResources", "SharedResources.InvalidQuestion");
                case UserCreateStatus.InvalidUserName:
                    return DNNrocketUtils.GetResourceString("/App_GlobalResources", "SharedResources.InvalidUserName");
                case UserCreateStatus.InvalidDisplayName:
                    return DNNrocketUtils.GetResourceString("/App_GlobalResources", "SharedResources.InvalidDisplayName");
                case UserCreateStatus.DuplicateDisplayName:
                    return DNNrocketUtils.GetResourceString("/App_GlobalResources", "SharedResources.DuplicateDisplayName");
                case UserCreateStatus.UserRejected:
                    return DNNrocketUtils.GetResourceString("/App_GlobalResources", "SharedResources.UserRejected");
                case UserCreateStatus.DuplicateUserName:
                case UserCreateStatus.UserAlreadyRegistered:
                case UserCreateStatus.UsernameAlreadyExists:
                    return DNNrocketUtils.GetResourceString("/App_GlobalResources", "SharedResources.UserNameExists");
                case UserCreateStatus.BannedPasswordUsed:
                    return DNNrocketUtils.GetResourceString("/App_GlobalResources", "SharedResources.BannedPasswordUsed");
                case UserCreateStatus.ProviderError:
                case UserCreateStatus.DuplicateProviderUserKey:
                case UserCreateStatus.InvalidProviderUserKey:
                    return DNNrocketUtils.GetResourceString("/App_GlobalResources", "SharedResources.RegError");
                case UserCreateStatus.Success:
                    return "";
                default:
                    throw new ArgumentException("Unknown UserCreateStatus value encountered", "userRegistrationStatus");
            }
        }

        public static int CreateUser(int portalId, string username, string email)
        {
            if (portalId >= 0 && username != "" && email != "")
            {
                UserInfo objUser = null;

                var userInfo = new UserInfo();
                userInfo.PortalID = portalId;
                userInfo.Username = username;
                userInfo.DisplayName = username;
                userInfo.Membership.Approved = true;
                userInfo.Membership.Password = UserController.GeneratePassword();
                userInfo.FirstName = username;
                userInfo.LastName = username;
                userInfo.Email = email;

                userInfo.Profile.PreferredLocale = DNNrocketUtils.GetCurrentCulture();
                userInfo.Profile.PreferredTimeZone = PortalSettings.Current.TimeZone;
                userInfo.Profile.FirstName = userInfo.FirstName;
                userInfo.Profile.LastName = userInfo.LastName;

                var status = UserController.CreateUser(ref userInfo);
                if (status == UserCreateStatus.Success
                    || status == UserCreateStatus.DuplicateUserName
                    || status == UserCreateStatus.UserAlreadyRegistered
                    || status == UserCreateStatus.UsernameAlreadyExists)
                {                    
                    objUser = UserController.GetUserByName(username);
                    UserController.AddUserPortal(portalId, objUser.UserID);
                }
                if (status == UserCreateStatus.DuplicateEmail)
                {
                    objUser = UserController.GetUserByEmail(portalId, email);
                    UserController.AddUserPortal(portalId, objUser.UserID);
                }
                if (objUser != null)
                {
                    var l = RoleController.Instance.GetRoles(portalId);
                    foreach (RoleInfo r in l)
                    {
                        if (r.AutoAssignment || r.RoleName == DNNrocketRoles.Manager)
                        {
                            UserUtils.AddUserRole(portalId, objUser.UserID, r.RoleID);
                        }
                    }
                    return objUser.UserID;
                }
            }
            return -1;
        }


        public static string GetUserEmail(int userId)
        {
            return GetUserEmail(PortalSettings.Current.PortalId, userId);
        }
        public static string GetUserEmail(int portalId, int userId)
        {
            if (userId <= 0) return "";
            var userInfo = UserController.GetUserById(portalId, userId);
            return userInfo.Email;
        }
        public static string GetUsername(int userId)
        {
            return GetUsername(PortalSettings.Current.PortalId, userId);
        }
        public static string GetUsername(int portalId, int userId)
        {
            if (userId <= 0) return "";
            var userInfo = UserController.GetUserById(portalId, userId);
            return userInfo.Username;
        }
        public static Dictionary<string, string> GetUserProfileProperties(string userId)
        {
            if (!GeneralUtils.IsNumeric(userId)) return null;
            var userInfo = UserController.GetUserById(PortalSettings.Current.PortalId, Convert.ToInt32(userId));
            return GetUserProfileProperties(userInfo);
        }
        private static Dictionary<string, string> GetUserProfileProperties(UserInfo userInfo)
        {
            var prop = new Dictionary<string, string>();
            foreach (DotNetNuke.Entities.Profile.ProfilePropertyDefinition p in userInfo.Profile.ProfileProperties)
            {
                prop.Add(p.PropertyName, p.PropertyValue);
            }
            if (!prop.ContainsKey("Email")) prop.Add("Email", userInfo.Email);
            if (!prop.ContainsKey("Username")) prop.Add("Username", userInfo.Username);
            if (!prop.ContainsKey("DisplayName")) prop.Add("DisplayName", userInfo.DisplayName);

            return prop;
        }
        private static void SetUserProfileProperties(UserInfo userInfo, Dictionary<string, string> properties)
        {
            foreach (var p in properties)
            {
                userInfo.Profile.SetProfileProperty(p.Key, p.Value);
                UserController.UpdateUser(PortalSettings.Current.PortalId, userInfo);
            }
        }
        public static void SetUserProfileProperties(String userId, Dictionary<string, string> properties)
        {
            if (GeneralUtils.IsNumeric(userId))
            {
                var userInfo = UserController.GetUserById(PortalSettings.Current.PortalId, Convert.ToInt32(userId));
                SetUserProfileProperties(userInfo, properties);
            }
        }

        public static int GetUserIdByUserName(int portalId, string username)
        {
            try
            {
                if (username == "") return -1;
                var objUser = UserController.GetUserByName(portalId, username);
                if (objUser != null) return objUser.UserID;
                return -1;
            }
            catch (Exception ex)
            {
                var ms = ex.ToString();
                return 0; // use zero;
            }
        }
        /// <summary>
        /// Return list of users in XML SimplisityRecord.  XML Format "user/email user/username user/userid user/displayname"
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="inRole"></param>
        /// <returns></returns>
        public static List<SimplisityRecord> GetUsers(int portalId, string inRole = "")
        {
            var rtnList = new List<SimplisityRecord>();
            var l = UserController.GetUsers(portalId);
            foreach (UserInfo u in l)
            {
                if (inRole == "" || u.IsInRole(inRole))
                {
                    var sRec = new SimplisityRecord();
                    sRec.SetXmlProperty("user/username", u.Username);
                    sRec.SetXmlProperty("user/email", u.Email);
                    sRec.SetXmlPropertyInt("user/userid", u.UserID.ToString());
                    sRec.SetXmlProperty("user/displayname", u.DisplayName);
                    rtnList.Add(sRec);
                }
            }
            return rtnList;
        }
        public static int GetUserIdByEmail(int portalId, string email)
        {
            try
            {
                if (email == "") return -1;
                var objUser = UserController.GetUserByEmail(portalId, email);
                if (objUser != null) return objUser.UserID;
                return -1;
            }
            catch (Exception ex)
            {
                var ms = ex.ToString();
                return 0; // use zero;
            }
        }

        public static bool IsInRole(string role)
        {
            return UserController.Instance.GetCurrentUserInfo().IsInRole(role);
        }

        public static bool IsSuperUser()
        {
            return UserController.Instance.GetCurrentUserInfo().IsSuperUser;
        }

        public static Boolean IsClientOnly()
        {
            if (UserController.Instance.GetCurrentUserInfo().IsInRole(DNNrocketRoles.ClientEditor) && (!UserController.Instance.GetCurrentUserInfo().IsInRole(DNNrocketRoles.Editor) && !UserController.Instance.GetCurrentUserInfo().IsInRole(DNNrocketRoles.Manager) && !UserController.Instance.GetCurrentUserInfo().IsInRole(DNNrocketRoles.Administrators)))
            {
                return true;
            }
            return false;
        }
        public static Boolean IsEditor()
        {
            if (UserController.Instance.GetCurrentUserInfo().IsInRole(DNNrocketRoles.ClientEditor) || 
                UserController.Instance.GetCurrentUserInfo().IsInRole(DNNrocketRoles.Editor) || 
                UserController.Instance.GetCurrentUserInfo().IsInRole(DNNrocketRoles.Manager) || 
                UserController.Instance.GetCurrentUserInfo().IsInRole(DNNrocketRoles.Administrators))
            {
                return true;
            }
            return false;
        }
        public static Boolean IsAdministrator()
        {
            if (UserController.Instance.GetCurrentUserInfo().IsInRole(DNNrocketRoles.Administrators)) return true;
            return false;
        }

        public static UserInfo GetValidUser(int PortalId, string username, string password)
        {
            var userLoginStatus = new UserLoginStatus();
            return UserController.ValidateUser(PortalId, username, password, "", "", "", ref userLoginStatus);
        }

        public static bool IsValidUser(int PortalId, string username)
        {
            var u = UserController.GetUserByName(PortalId, username);
            if (u != null)
            {
                return true;
            }
            return false;
        }
        public static bool IsValidUser(int PortalId, string username, string password)
        {
            var u = GetValidUser(PortalId, username, password);
            if (u != null)
            {
                return true;
            }
            return false;
        }
        public static bool IsValidUser(int portalId, int userId)
        {
            var u = UserController.GetUserById(portalId, userId);
            if (u != null)
            {
                return true;
            }
            return false;
        }

        public static SimplisityRecord GetRoleById(int portalId, int roleId)
        {
            var r = RoleController.Instance.GetRoleById(portalId, roleId);
            var rtnRec = new SimplisityRecord();
            rtnRec.ItemID = roleId;
            rtnRec.SetXmlProperty("genxml/rolename", r.RoleName);
            rtnRec.SetXmlProperty("genxml/roleid", roleId.ToString());
            return rtnRec;
        }

        public static Dictionary<int, string> GetRoles(int portalId)
        {
            var rtnDic = new Dictionary<int, string>();
            var l = RoleController.Instance.GetRoles(portalId);
            foreach (RoleInfo r in l)
            {
                rtnDic.Add(r.RoleID, r.RoleName);
            }
            return rtnDic;
        }

        public static void AddUserRole(int portalId, int userId, int roleId)
        {
            var u = UserController.GetUserById(portalId, userId);
            if (u != null)
            {
                RoleController.Instance.AddUserRole(portalId, userId, roleId, RoleStatus.Approved, false, Null.NullDate, Null.NullDate);
            }
        }
        public static void SignOut()
        {
            var ps = new PortalSecurity();
            ps.SignOut();
        }

    }
}
