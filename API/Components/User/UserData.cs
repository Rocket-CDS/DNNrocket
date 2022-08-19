using DotNetNuke.Entities.Users;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Components
{
    public class UserData
    {
        public UserData()
        {
        }

        #region "properties"

        public SimplisityInfo Info { get; set; }
        public int UserId { get; set; }
        public int PortalId { get; set; }

        public string Email { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }        

        public DateTime CreatedOnDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime LastPasswordChangeDate { get; set; }

        public bool IsLockedOut { get; set; }
        public bool Approved { get; set; }
        public bool IsDeleted { get; set; }
        public bool Exists { get; set; }

        #endregion


    }
}
