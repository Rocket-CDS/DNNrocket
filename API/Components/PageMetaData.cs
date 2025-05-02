using DotNetNuke.Abstractions;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.HtmlControls;

namespace DNNrocketAPI.Components
{
    public class PageMetaData
    {
        public PageMetaData()
        {
            CssRemovalPattern = new List<string>();
            HtmlMeta = new List<HtmlMeta>();
        }
        public string Language { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string KeyWords { get; set; }
        public string CanonicalLinkUrl { get; set; }
        public string AlternateLinkHtml { get; set; }
        public List<HtmlMeta> HtmlMeta { get; set; }
        public bool Redirect404 { get; set; }
        public List<string> CssRemovalPattern { get; set; }

    }
}
