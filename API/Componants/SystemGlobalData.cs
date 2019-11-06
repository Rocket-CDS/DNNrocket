using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Componants
{
    public class SystemGlobalData
    {
        private static DNNrocketController _objCtrl;
        public SystemGlobalData()
        {
            _objCtrl = new DNNrocketController();
            LoadData();
        }

        public void Save(SimplisityInfo postInfo)
        {
            Info.XMLData = postInfo.XMLData;
            _objCtrl.Update(Info);
        }

        private void LoadData()
        {
            Info = _objCtrl.GetByType(DNNrocketUtils.GetPortalId(), -1, "GLOBALSETTINGS");
            if (Info == null)
            {
                Info = new SimplisityInfo();
                Info.ItemID = -1;
                Info.PortalId = DNNrocketUtils.GetPortalId();
                Info.TypeCode = "GLOBALSETTINGS";
                Info.ItemID = _objCtrl.Update(Info);
            }
        }

        public SimplisityInfo Info { get; set; }

        public string FtpUserName { get { return Info.GetXmlProperty("genxml/textbox/ftpuser"); } }
        public string FtpPassword { get { return Info.GetXmlProperty("genxml/textbox/ftppassword"); } }
        public string FtpServer { get { return Info.GetXmlProperty("genxml/textbox/ftpserver"); } }
        public string PublicAppThemeURI { get { return Info.GetXmlProperty("genxml/textbox/publicappthemeuri"); } }


    }
}
