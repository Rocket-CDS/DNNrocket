using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Search.Entities;
using DotNetNuke.Services.Search.Internals;
using DotNetNuke.Web.DDRMenu;
using Newtonsoft.Json;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace DNNrocketAPI.Components
{
    public static class SearchUtils
    {

        public static List<string> DoSearch(int portalId, SearchPostModel model)
        {
            var searchTypes = SearchHelper.Instance.GetSearchTypes().Select(st => st.SearchTypeId).ToArray();
            var searchQuery = new SearchQuery()
            {
                WildCardSearch = true,
                KeyWords = model.SearchInput,
                SearchTypeIds = searchTypes,
                PortalIds = new[] { portalId },
                PageSize = model.PageSize,
                PageIndex = model.PageIndex,
            };
            if (!string.IsNullOrEmpty(model.SearchTags))
            {
                searchQuery.Tags = model.SearchTags?.Replace("\r\n", "\n").Split('\n');
            }
            var ret = new List<string>();
            try
            {
                var res = DotNetNuke.Services.Search.Controllers.SearchController.Instance.SiteSearch(searchQuery);
                foreach (var r in res.Results)
                {
                    ret.Add(r.UniqueKey);
                }
            }
            catch (Exception e)
            {
                LogUtils.LogSystem("ERROR - SearchUtils.DoSearch: " + e.ToString());
                LogUtils.LogException(e);
            }
            return ret;
        }
        public static List<SimplisityRecord> DoSearchRecord(int portalId, SearchPostModel model)
        {
            var searchTypes = SearchHelper.Instance.GetSearchTypes().Select(st => st.SearchTypeId).ToArray();
            var searchQuery = new SearchQuery()
            {
                WildCardSearch = true,
                KeyWords = model.SearchInput,
                SearchTypeIds = searchTypes,
                PortalIds = new[] { portalId },
                PageSize = model.PageSize,
                PageIndex = model.PageIndex,
            };
            if (!string.IsNullOrEmpty(model.SearchTags))
            {
                searchQuery.Tags = model.SearchTags?.Replace("\r\n", "\n").Split('\n');
            }
            var ret = new List<SimplisityRecord>();
            try
            {
                var res = DotNetNuke.Services.Search.Controllers.SearchController.Instance.SiteSearch(searchQuery);
                foreach (var r in res.Results)
                {
                    var requestJson = "{'?xml': {'@version': '1.0','@standalone': 'no'},'root' :" + r.ToJsonString() + "}";
                    XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode(requestJson);
                    var sr = new SimplisityRecord();
                    sr.XMLData = doc.OuterXml;
                    ret.Add(sr);
                }
            }
            catch (Exception e)
            {
                LogUtils.LogSystem("ERROR - SearchUtils.DoSearch: " + e.ToString());
                LogUtils.LogException(e);
            }
            return ret;
        }
        public static void DeleteAllDocuments(int portalId)
        {
            InternalSearchController.Instance.DeleteAllDocuments(portalId, SearchHelper.Instance.GetSearchTypeByName("module").SearchTypeId);
        }
        public static void DeleteModuleDocuments(int portalId, int moduleId)
        {
            InternalSearchController.Instance.DeleteSearchDocumentsByModule(portalId, moduleId, SearchHelper.Instance.GetSearchTypeByName("module").SearchTypeId);
        }

    }
    public class SearchPostModel
    {
        public string SearchInput { get; set; }
        public string SearchTags { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }

}
