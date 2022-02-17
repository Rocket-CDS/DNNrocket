using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketComm
{
    public class CommData
    {
        public CommData()
        {
            StatusCode = "";
            ErrorMsg = "";
            FirstHeader = "";
            LastHeader = "";
            SeoHeader = new MetaSEO();
            ViewHtml = "";
            JsonReturn = "{\"\":\"\"}";
            CacheFlag = false;
        }

        public string StatusCode { set; get; }
        public string ErrorMsg { set; get; }
        public string FirstHeader { set; get; }
        public string LastHeader { set; get; }
        public MetaSEO SeoHeader { set; get; }
        public string ViewHtml { set; get; }
        public string JsonReturn { set; get; }
        public bool CacheFlag { set; get; }

    }
}
