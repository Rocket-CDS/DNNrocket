using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Search.Entities;
using DotNetNuke.Services.Search.Internals;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
    public class SearchPostModel
    {
        public string SearchInput { get; set; }
        public string SearchTags { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }

}
