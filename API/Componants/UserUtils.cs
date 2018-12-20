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
using ICSharpCode.SharpZipLib.Zip;

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

    }
}
