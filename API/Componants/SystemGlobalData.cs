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
        private static string _cacheKey; 
        public SystemGlobalData(bool cache = true)
        {
            _cacheKey = "rocketGLOBALSETTINGS";
            if (cache) Info = (SimplisityInfo)CacheUtils.GetCache(_cacheKey);
            if (Info == null)
            {
                LoadData();
            }
        }

        public void Save(SimplisityInfo postInfo)
        {
             var objCtrl = new DNNrocketController();
            Info.XMLData = postInfo.XMLData;
            objCtrl.Update(Info);
            CacheUtils.SetCache(_cacheKey, Info);
        }
        public void Update()
        {
            var objCtrl = new DNNrocketController();
            objCtrl.Update(Info);
            CacheUtils.SetCache(_cacheKey, Info);
        }

        private void LoadData()
        {
            var objCtrl = new DNNrocketController();
            Info = objCtrl.GetByType(DNNrocketUtils.GetPortalId(), -1, "GLOBALSETTINGS");
            if (Info == null)
            {
                Info = new SimplisityInfo();
                Info.ItemID = -1;
                Info.PortalId = DNNrocketUtils.GetPortalId();
                Info.TypeCode = "GLOBALSETTINGS";
                Info.ItemID = objCtrl.Update(Info);
            }
        }

        public SimplisityInfo Info { get; set; }

        public string FtpUserName { get { return Info.GetXmlProperty("genxml/textbox/ftpuser"); } set { Info.SetXmlProperty("genxml/textbox/ftpuser",value); } }
        public string FtpPassword { get { return Info.GetXmlProperty("genxml/textbox/ftppassword"); } set { Info.SetXmlProperty("genxml/textbox/ftppassword", value); } }
        public string FtpServer { get { return Info.GetXmlProperty("genxml/textbox/ftpserver"); } set { Info.SetXmlProperty("genxml/textbox/ftpserver", value); } }
        public string PublicAppThemeURI { get { return Info.GetXmlProperty("genxml/textbox/publicappthemeuri"); } set { Info.SetXmlProperty("genxml/textbox/publicappthemeuri", value); } }
        public string ImageType { get { return Info.GetXmlProperty("genxml/select/imagetype"); } set { Info.SetXmlProperty("genxml/select/imagetype", value); } }
        public bool PngImage { get { if (Info.GetXmlProperty("genxml/select/imagetype") != "jpg") return true; else return false; } }

    }
}
