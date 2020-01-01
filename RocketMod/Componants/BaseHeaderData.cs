using DNNrocketAPI;
using Simplisity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace RocketMod
{

    public class BaseHeaderData
    {
        private string _entityTypeCode;
        private SimplisityInfo _header;

        #region "properties"
        public string EntityTypeCode { get { return _entityTypeCode; } set { _entityTypeCode = value; } }

        public int PageSize
        {
            get {
                var ps = _header.GetXmlPropertyInt("genxml/hidden/pagesize");
                if (ps == 0) ps = 20;
                return ps;
            }
            set {
                var v = value;
                if (v == 0) v = 20;
                _header.SetXmlProperty("genxml/hidden/pagesize", v.ToString());
            }
        }
        public int Page
        {
            get
            {
                var ps = _header.GetXmlPropertyInt("genxml/hidden/page");
                if (ps == 0) ps = 1;
                return ps;
            }
            set
            {
                var v = value;
                if (v == 0) v = 1;
                _header.SetXmlProperty("genxml/hidden/page", v.ToString());
            }
        }

        public int RowCount
        {
            get
            {
                var ps = _header.GetXmlPropertyInt("genxml/hidden/rowcount");
                return ps;
            }
            set
            {
                _header.SetXmlProperty("genxml/hidden/rowcount", value.ToString());
            }
        }
        public string SearchText
        {
            get
            {
                return _header.GetXmlProperty("genxml/textbox/searchtext");
            }
            set
            {
                _header.SetXmlProperty("genxml/textbox/searchtext", value.ToString());
            }
        }
        public bool ShowArchived
        {
            get
            {
                return _header.GetXmlPropertyBool("genxml/checkbox/showarchive");
            }
            set
            {
                _header.SetXmlProperty("genxml/checkbox/showarchive", value.ToString());
            }
        }

        public SimplisityInfo Header
        {
            get { return _header;}
            set {_header = value; }
        }

        #endregion



    }

}
