using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Componants
{
    public class UserLimpet
    {
        public UserLimpet(int portalId, int userId)
        {
            PortalId = portalId;
            UserId = userId;
            Populate();
        }
        public UserLimpet(int portalId, string username)
        {
            PortalId = portalId;
            UserId = UserUtils.GetUserIdByUserName(portalId, username);
            Populate();
        }
        public UserLimpet(string email, int portalId)
        {
            PortalId = portalId;
            UserId = UserUtils.GetUserIdByEmail(portalId, email);
            Populate();
        }

        private void Populate()
        {

        }

        #region "properties"

        public SimplisityInfo Info { get; set; }
        public int UserId { get; set; }
        public int PortalId { get; set; }


        #endregion


    }
}
