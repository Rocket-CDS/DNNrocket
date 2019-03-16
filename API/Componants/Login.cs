using DNNrocketAPI;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Entities.Users.Membership;
using DotNetNuke.Security.Membership;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Componants
{
    public class LoginUtils
    {

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

        public static string DoLogin(SimplisityInfo sInfo, string userHostAddress)
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
            strOut = LoginForm(rtnInfo,"login", objUser.UserID);

            return strOut;
        }

        public static string LoginForm(SimplisityInfo sInfo, string interfacekey,int userid)
        {
            if (userid > 0)
            {
                sInfo.SetXmlProperty("genxml/securityaccess", "You do not have security access");
            }
            var razorTempl = DNNrocketUtils.GetRazorTemplateData("LoginForm.cshtml", "/DesktopModules/DNNrocket/API", "config-w3", DNNrocketUtils.GetCurrentCulture());
            sInfo.SetXmlProperty("genxml/interfacekey", interfacekey); // make sure the login form has the correct interface command.
            return DNNrocketUtils.RazorDetail(razorTempl, sInfo);
        }

    }
}
